using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetNote.Models;
using System.Threading.Tasks;
using NetNote.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace NetNote.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public UserManager<NoteUser> UserManager { get; }

        public SignInManager<NoteUser> SignInManager { get; }

        private readonly IMapper _mapper;

        public AccountController(UserManager<NoteUser> userManager, SignInManager<NoteUser> signInManager, ILogger<AccountController> logger, IMapper iMapper)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _logger = logger;
            _mapper = iMapper;
        }
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RemeberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            //if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claims = new Claim[]{
                    new Claim("UserName",model.UserName)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                _logger.LogInformation("Logged in {userName}.", model.UserName);
                return RedirectToAction("Index", "Note");
            }
            else
            {
                _logger.LogWarning("Failed to log in {userName}", model.UserName);
                ModelState.AddModelError("", "用户名或密码错误");
                return View(model);
            }
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<NoteUser>(model);
                //var user = new NoteUser { UserName = model.UserName, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {userName} was created", model.Email);
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            var username = HttpContext.User.Identity.Name;
            await SignInManager.SignOutAsync();
            _logger.LogInformation("{userName} logged out.", username);
            return RedirectToAction("Index", "Home");
        }
    }
}
