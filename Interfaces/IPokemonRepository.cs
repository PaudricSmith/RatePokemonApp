using RatePokemonApp.Models;

namespace RatePokemonApp.Interfaces
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllPokemonAsync();
        Task<Pokemon> GetPokemonByIdAsync(int id);
        Task<Pokemon> GetPokemonByNameAsync(string name);
        Task<decimal> GetPokemonAverageRatingAsync(int pokemonId);
        Task<bool> PokemonExistsAsync(int pokemonId);
        Task<bool> CreatePokemonAsync(int ownerId, int categoryId, Pokemon pokemon);
        Task<bool> UpdatePokemonAsync(Pokemon pokemon);
        Task<bool> DeletePokemonAsync(Pokemon pokemon);
        Task<bool> SaveAsync();
    }
}
