namespace BookStore.Service.Category
{
    public interface ICategoryService
    {
        public Task AddCategory(Models.Category category);
        public Task<List<Models.Category>> GetAllCategories();
        public Task DeleteCategory(int id);

    }
}
