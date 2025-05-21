using Microsoft.AspNetCore.Identity;

namespace BookStore.Service.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<Models.User> _userManager;
        public UserService(UserManager<Models.User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Models.User>> GetAllUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return (List<Models.User>)users;
        }

        public async Task<bool> BanUser(string userId, DateTime dateTime)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                if (dateTime < DateTime.Now)
                    return false;
                user.LockoutEnd = dateTime;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<bool> UnBanUser(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LockoutEnd = null;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            return false;
        }
    }
}
