using BookStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly BookStoreDBContext _context;
        public CategoryService(BookStoreDBContext context)
        {
            _context = context;
        }

        public async Task AddCategory(Models.Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
