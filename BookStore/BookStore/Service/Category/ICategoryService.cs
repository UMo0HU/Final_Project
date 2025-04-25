namespace BookStore.Service.Category
{
    public interface ICategoryService
    {
        public Task AddCategory(Models.Category category);
        public Task<List<Models.Category>> GetAllCategories();
        public Task<Models.Category> GetCategoryById(int id);

        public Task EditCategory(Models.Category updatedCategory);
        public Task DeleteCategory(int id);

    }
}
