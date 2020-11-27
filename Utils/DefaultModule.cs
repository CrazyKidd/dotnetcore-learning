using System;
using Autofac;
using CSRedis;
using NetNote.Models;
using RedLockNet;
using RedLockNet.SERedis;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using RedLockNet.SERedis.Configuration;
using AutoMapper;
using NetNote.AutoMapper;
using NetNote.Aop;

namespace NetNote.Utils
{
    public class DefaultModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //InstancePerLifetimeScope：同一个Lifetime生成的对象是同一个实例
            //SingleInstance：单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
            //InstancePerDependency：默认模式，每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
            // services.AddScoped<INoteRepository, NoteRepository>();
            // services.AddScoped<INoteTypeRepository, NoteTypeRepository>();
            // services.AddSingleton(typeof(IDistributedLockFactory), lockFactory);
            // services.AddScoped(typeof(ProductService));
            // services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));
            // services.AddScoped<IResdisClient, CustomerRedis>();
            builder.RegisterType<LogInterceptor>();
            builder.RegisterType<NoteRepository>().As<INoteRepository>().InstancePerLifetimeScope();
            builder.RegisterType<NoteTypeRepository>().As<INoteTypeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRedis>().As<IResdisClient>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().InstancePerLifetimeScope();
            //services.AddSingleton(typeof(IDistributedLockFactory), lockFactory);
            builder.RegisterInstance(lockFactory).As<IDistributedLockFactory>().SingleInstance();
            builder.Register(
                c => new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new AutoMapperConfigs());
                }))
                .AsSelf()
                .SingleInstance();

            builder.Register(
                c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }

        private RedLockFactory lockFactory
        {
            get
            {
                string[] redisUrl = ConfigHelper.GetSection("RedisUrls").GetChildren().Select(x => x.Value).ToArray();
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
