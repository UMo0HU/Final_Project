namespace BookStore.Service.User
{
    public interface IUserService
    {
        public Task<List<Models.User>> GetAllUsers();
        public Task<bool> BanUser(string userId, DateTime dateTime);
        public Task<bool> UnBanUser(string userId);
    }
}
