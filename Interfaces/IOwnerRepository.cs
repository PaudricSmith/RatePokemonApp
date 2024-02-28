using RatePokemonApp.Models;

namespace RatePokemonApp.Interfaces
{
    public interface IOwnerRepository
    {
        Task<ICollection<Owner>> GetAllOwnersAsync();
        Task<ICollection<Owner>> GetOwnerByPokemonIdAsync(int pokemonId);
        Task<ICollection<Pokemon>> GetPokemonByOwnerIdAsync(int ownerId);
        Task<Owner> GetOwnerByIdAsync(int ownerId);
        Task<Owner> GetOwnerByNameAsync(string name);
        Task<bool> CreateOwnerAsync(Owner owner);
        Task<bool> UpdateOwnerAsync(Owner owner);
        Task<bool> DeleteOwnerAsync(Owner owner);
        Task<bool> OwnerExistsAsync(int ownerId);
        Task<bool> SaveAsync();
    }
}
