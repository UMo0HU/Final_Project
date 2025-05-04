using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Net;

namespace BookStore.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var existingUsername = await _userManager.FindByNameAsync(model.Name);
            if (existingUsername != null)
            {
                throw new ArgumentException("Username already Exist.", nameof(model.Name));
            }
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                throw new ArgumentException("Email already Exist.", nameof(model.Email));
            }

            var user = new User
            {
                UserName = model.Name,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            return result;
        }

        public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return SignInResult.Failed;
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);
            return result;
        }

        public async Task<string> GetUserId()
        {
            var user = _signInManager.Context.User;
            if (user != null && user.Identity.IsAuthenticated)
            {
                return _userManager.GetUserId(user);
            }
            return null;
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> AccountExist(string email)
        {
            if((await _userManager.FindByEmailAsync(email)) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<string> GenerateForgetPasswordTokenEncoded(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return WebUtility.UrlEncode(token);
        }

        public async Task<bool> ResetPassword(string email, string encodedToken, string newPassword)
        {
            string token = WebUtility.UrlDecode(encodedToken);
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded) return true;
            return false;
        }

    }
}
