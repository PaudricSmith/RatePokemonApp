using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;

namespace RatePokemonApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<ICollection<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.OrderBy(r => r.Id).ToListAsync();
        }

        public async Task<ICollection<Review>> GetReviewsByPokemonIdAsync(int pokemonId)
        {
            return await _context.Reviews.Where(r => r.Pokemon.Id == pokemonId).ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefaultAsync();
        }

        public async Task<Review> GetReviewByNameAsync(string title)
        {
            return await _context.Reviews.Where(r => r.Title == title).FirstOrDefaultAsync();
        }

        public async Task<bool> ReviewExistsAsync(int reviewId)
        {
            return await _context.Reviews.AnyAsync(r => r.Id == reviewId);
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            _context.Add(review);

            return await SaveAsync();
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);

            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);

            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewsAsync(List<Review> reviews)
        {
            _context.Reviews.RemoveRange(reviews);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
