using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;
using System.Xml.Linq;

namespace RatePokemonApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {

        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Retrieves all categories ordered by their ID.
        /// </summary>
        /// <returns>A collection of all categories.</returns>
        public async Task<ICollection<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<ICollection<Pokemon>> GetPokemonByCategoryIdAsync(int categoryId)
        {
            return await _context.PokemonCategories
                .Where(pc => pc.CategoryId == categoryId)
                .Select(p => p.Pokemon).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .Where(c => c.Name.Trim().ToUpper() == name.TrimEnd().ToUpper())
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.Id == categoryId);
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            _context.Add(category);

            return await SaveAsync();
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);

            return await SaveAsync();
        }

        public async Task<bool> DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
