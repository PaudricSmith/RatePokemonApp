using RatePokemonApp.Models;

namespace RatePokemonApp.Interfaces
{
    public interface IReviewRepository
    {
        Task<ICollection<Review>> GetAllReviewsAsync();
        Task<ICollection<Review>> GetReviewsByPokemonIdAsync(int pokemonId);
        Task<Review> GetReviewByIdAsync(int reviewId);
        Task<Review> GetReviewByNameAsync(string name);
        Task<bool> ReviewExistsAsync(int reviewId);
        Task<bool> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(Review review);
        Task<bool> DeleteReviewsAsync(List<Review> reviews);
        Task<bool> SaveAsync();
    }
}
