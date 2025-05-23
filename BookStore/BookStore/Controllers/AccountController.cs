using BookStore.Models;
using BookStore.Service.Account;
using BookStore.Service.Email;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Runtime.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSenderService _emailSenderService;

        public AccountController(IAccountService accountService, IEmailSenderService emailSenderService)
        {
            _accountService = accountService;
            _emailSenderService = emailSenderService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.LoginUserAsync(loginViewModel);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (User.IsInRole("User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }
            return View(loginViewModel);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _accountService.RegisterUserAsync(registerViewModel);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(registerViewModel);
                    }
                }
                catch(ArgumentException ex)
                {
                    
                    ModelState.AddModelError(ex.ParamName, ex.Message.Split('(')[0]);
                    return View(registerViewModel);
                }

            }
            return View(registerViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutUserAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var accountExist = await _accountService.AccountExist(model.Email);
                if(!accountExist)
                {
                    ModelState.AddModelError(string.Empty, "Email does not exist.");
                    return View(model);
                }

                string encodedToken = await _accountService.GenerateForgetPasswordTokenEncoded(model.Email);

                var resetLink = Url.Action(
                    action: "ResetPassword",
                    controller: "Account",
                    values: new { email = model.Email, encodedToken = encodedToken },
                    protocol: Request.Scheme
                );
                string subject = "Password Reset Request";
                string message = $@"
                    <img src=""https://i.ibb.co/sdt2Qngr/smile.jpg"" alt=""reset-Password-Image"" border=""0"" width=""200"" height=""200"">
                    <p>This Link Is To Reset Your Password, <a href='{resetLink}'>Click Me</a> If You Want To Reset IT.</p>
                    <p>This Link Expires In 1 Hour.</p>
                ";

                await _emailSenderService.SendEmailAsync(model.Email, subject, message, true);

                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string encodedToken)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = email,
                Token = encodedToken
            };

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _accountService.ResetPassword(model.Email, model.Token, model.Password);
                if (result)
                {
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError(string.Empty, "Operation Couldn't Be Completed");
                return View(model);
            }

            return View(model);
        }
    }
}
