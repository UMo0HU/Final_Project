using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System.Net;
using System.Security.Claims;

namespace BookStore.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Models.User> _userManager;
        private readonly SignInManager<Models.User> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        public AccountService(UserManager<Models.User> userManager, SignInManager<Models.User> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var existingUsername = await _userManager.FindByNameAsync(model.Username);
            if (existingUsername != null)
            {
                throw new ArgumentException("Username already Exist.", nameof(model.Username));
            }
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                throw new ArgumentException("Email already Exist.", nameof(model.Email));
            }

            var user = new Models.User
            {
                UserName = model.Username,
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

        public async Task<bool> ChangePassword(string currentPassword, string newPassword)
        {
            if(_user != null && _user.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_user);
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if(result.Succeeded)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        public async Task<bool> ChangeProfilePicture(IFormFile profilePicture)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_user);
                if (profilePicture == null || profilePicture.Length == 0)
                {
                    return false;
                }
                using (var memoryStream = new MemoryStream())
                {
                    await profilePicture.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 2097152)
                    {
                        user.ProfilePicture = memoryStream.ToArray();
                        var result = await _userManager.UpdateAsync(user);
                        if(result.Succeeded)
                        {
                            return true;
                        }
                        throw new ArgumentException("Error Has Happened.", nameof(profilePicture));
                    }
                    throw new ArgumentException("The File Is Too Large.", nameof(profilePicture));
                }
            }
            return false;
        }

        public async Task<bool> ChangeUsername(string newUsername)
        {
            if (_user != null && _user.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_user);
                if(newUsername != null && newUsername != user.UserName)
                {
                    user.UserName = newUsername;
                    user.NormalizedUserName = newUsername.ToUpper();

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }
    }
}
