using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;

namespace RatePokemonApp.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;

        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Reviewer>> GetAllReviewersAsync()
        {
            return await _context.Reviewers.OrderBy(r => r.Id).ToListAsync();
        }
        
        public async Task<IEnumerable<Review>> GetReviewsByReviewerAsync(int reviewerId)
        {
            return await _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToListAsync();
        }

        public async Task<Reviewer> GetReviewerByIdAsync(int reviewerId)
        {
            return await _context.Reviewers.Where(r => r.Id == reviewerId).Include(r => r.Reviews).FirstOrDefaultAsync();
        }

        public async Task<Reviewer> GetReviewerByNameAsync(string lastName)
        {
            return await _context.Reviewers.Where(r => r.LastName == lastName).FirstOrDefaultAsync();
        }

        public async Task<bool> ReviewerExistsAsync(int reviewerId)
        {
            return await _context.Reviewers.AnyAsync(r => r.Id == reviewerId);
        }

        public async Task<bool> CreateReviewerAsync(Reviewer reviewer)
        {
            _context.Add(reviewer);

            return await SaveAsync();
        }

        public async Task<bool> UpdateReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Update(reviewer);

            return await SaveAsync();
        }

        public async Task<bool> DeleteReviewerAsync(Reviewer reviewer)
        {
            _context.Reviewers.Remove(reviewer);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
