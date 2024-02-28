using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;

namespace RatePokemonApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<ICollection<Country>> GetAllCountriesAsync()
        {
            return await _context.Countries.OrderBy(c => c.Id).ToListAsync();
        }
        public async Task<ICollection<Owner>> GetOwnersByCountryIdAsync(int countryId)
        {
            return await _context.Owners.Where(o => o.Country.Id == countryId).ToListAsync();
        }

        public async Task<bool> CountryExistsAsync(int id)
        {
            return await _context.Countries.AnyAsync(c => c.Id == id);
        }

        public async Task<Country> GetCountryByIdAsync(int id)
        {
            return await _context.Countries.Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Country> GetCountryByNameAsync(string name)
        {
            return await _context.Countries
                .Where(c => c.Name.Trim().ToUpper() == name.TrimEnd().ToUpper())
                .FirstOrDefaultAsync();
        }

        public async Task<Country> GetCountryByOwnerIdAsync(int ownerId)
        {
            return await _context.Owners.Where(o => o.Id == ownerId).Select(c => c.Country).FirstOrDefaultAsync(); 
        }

        public async Task<bool> CreateCountryAsync(Country country)
        {
            _context.Add(country);

            return await SaveAsync();
        }

        public async Task<bool> UpdateCountryAsync(Country country)
        {
            _context.Countries.Update(country);

            return await SaveAsync();
        }

        public async Task<bool> DeleteCountryAsync(Country country)
        {
            _context.Countries.Remove(country);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
