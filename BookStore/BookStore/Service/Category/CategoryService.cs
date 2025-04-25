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

        public async Task<Models.Category> GetCategoryById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task EditCategory(Models.Category updatedCategory)
        {
            var category = await _context.Categories.FindAsync(updatedCategory.Id);
            if (category != null)
            {
                category.Name = updatedCategory.Name;
                await _context.SaveChangesAsync();
            } 
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if(category != null)
            {
                var relatedBookCategories = _context.BookCategories.Where(bc => bc.CategoryId == id);
                _context.BookCategories.RemoveRange(relatedBookCategories);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
