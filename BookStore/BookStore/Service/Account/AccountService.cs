using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;

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
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Username is already taken."
                });
            }
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email is already taken."
                });
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
    }
}
