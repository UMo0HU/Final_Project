using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Service.Account
{
    public interface IAccountService
    {
        public Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        public Task<SignInResult> LoginUserAsync(LoginViewModel model);
        public Task LogoutUserAsync();
        public Task<string> GetUserId();


    }
}
