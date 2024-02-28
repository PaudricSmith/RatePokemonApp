using RatePokemonApp.Models;

namespace RatePokemonApp.Interfaces
{
    public interface IReviewerRepository
    {
        Task<IEnumerable<Reviewer>> GetAllReviewersAsync();
        Task<IEnumerable<Review>> GetReviewsByReviewerAsync(int reviewerId);
        Task<Reviewer> GetReviewerByIdAsync(int reviewerId);
        Task<Reviewer> GetReviewerByNameAsync(string name);
        Task<bool> ReviewerExistsAsync(int reviewerId);
        Task<bool> CreateReviewerAsync(Reviewer reviewer);
        Task<bool> UpdateReviewerAsync(Reviewer reviewer);
        Task<bool> DeleteReviewerAsync(Reviewer reviewer);
        Task<bool> SaveAsync();
    }
}
