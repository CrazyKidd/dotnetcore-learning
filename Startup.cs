using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using NetNote.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Linq;
using System.Collections.Generic;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Net;
using RedLockNet;
using NetNote.Api;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using CSRedis;
using NetNote.Utils;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using NetNote.Filter;
using NLog.Extensions.Logging;
using NetNote.AutoMapper;

namespace NetNote
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper._Init(configuration);
        }

        public IConfiguration Configuration
        {
            get;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CSRedisClient csredis = new CSRedis.CSRedisClient(Configuration.GetSection("RedisConnectionString")["Default"]);
            //var connection = @"Server=.;Database=Note;UID=sa;PWD=12345678;";
            services.AddDbContext<NoteContext>(options =>
            {

                //options.UseSqlServer(connection)
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });
            services.AddIdentity<NoteUser, IdentityRole>()
                .AddEntityFrameworkStores<NoteContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                //options.Cookies.ApplicationCookie.LoginPath = new PathString("/Account/Login");
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Home/Index");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.Name = "CookieName";
                });

            services.AddControllersWithViews();
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));
            });
            //允许所有跨域
            services.AddCors(options => options.AddPolicy("cors", policy =>
            {
                policy
                .SetIsOriginAllowed(x => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            }));
            services.AddMvc(option => option.EnableEndpointRouting = false);
            //允许一个或多个来源可以跨域
            // services.AddCors(options =>
            // {
            //     options.AddPolicy("CustomCorsPolicy", policy =>{
            //         // 设定允许跨域的来源，有多个可以用','隔开
            //         policy.WithOrigins("http://localhost:8081","http://localhost:8082")
            //         .AllowAnyHeader()
            //         .AllowAnyMethod()
            //         .AllowCredentials();
            //       });
            // });
            services.AddHttpClient();
            services.AddAutoMapper(typeof(AutoMapperConfigs));
            // services.AddScoped<INoteRepository, NoteRepository>();
            // services.AddScoped<INoteTypeRepository, NoteTypeRepository>();
            //services.AddSingleton(typeof(IDistributedLockFactory), lockFactory);
            // services.AddScoped(typeof(ProductService));
            services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));
            //services.AddScoped<IResdisClient, CustomerRedis>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //添加依赖注入关系
            builder.RegisterModule(new DefaultModule());
            var controllerBaseType = typeof(ControllerBase);
            //在控制器中使用依赖注入
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
             .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
             .PropertiesAutowired();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime lifeTime)
        {
            InitData(app.ApplicationServices);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseBasicMiddleware(new BasicUser { UserName = "admin", Password = "123456" });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseCors()必须放在app.UseMvc()之前。
            app.UseCors("cors");
            // app.UseMvc(
            //     routes=>{
            //        routes.MapRoute("default","{controller=Home}/{action=Index}/{id?}"); 
            //     }
            // );
            //先验证后授权
            //验证
            app.UseAuthentication();

            //授权
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            lifeTime.ApplicationStopped.Register(() =>
            {
                lockFactory.Dispose();
            });
        }

        private void InitData(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<NoteContext>();
                db.Database.EnsureCreated();
                if (db.NoteTypes.Count() == 0)
                {
                    var NoteTypes = new List<NoteType>() {
                        new NoteType {
                            Name = "日常记录"
                        },
                        new NoteType {
                            Name = "代码收藏"
                        },
                        new NoteType {
                            Name = "消费记录"
                        },
                        new NoteType {
                            Name = "网站收藏"
                        }
                    };
                    db.NoteTypes.AddRange(NoteTypes);
                    db.SaveChanges();
                }
            }
        }

        private RedLockFactory lockFactory
        {
            get
            {
                string[] redisUrl = Configuration.GetSection("RedisUrls").GetChildren().Select(x => x.Value).ToArray();
                if (redisUrl.Length <= 0)
                {
                    throw new ArgumentException("RedisUrl 不能为空");
                }
                var endpoints = new List<RedLockEndPoint>();
                foreach (var item in redisUrl)
                {
                    var arr = item.Split(":");
                    endpoints.Add(new DnsEndPoint(arr[0], Convert.ToInt32(arr[1])));
                }
                return RedLockFactory.Create(endPoints: endpoints);
            }
        }
    }
}