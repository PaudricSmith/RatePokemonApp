using RatePokemonApp.Models;

namespace RatePokemonApp.Interfaces
{
    public interface ICountryRepository
    {
        Task<ICollection<Country>> GetAllCountriesAsync();
        Task<ICollection<Owner>> GetOwnersByCountryIdAsync(int countryId);
        Task<Country> GetCountryByOwnerIdAsync(int ownerId);
        Task<Country> GetCountryByIdAsync(int id);
        Task<Country> GetCountryByNameAsync(string name);
        Task<bool> CreateCountryAsync(Country country);
        Task<bool> UpdateCountryAsync(Country country);
        Task<bool> DeleteCountryAsync(Country country);
        Task<bool> CountryExistsAsync(int id);
        Task<bool> SaveAsync();
    }
}
