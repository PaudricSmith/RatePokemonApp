using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;
using System.Diagnostics.Metrics;

namespace RatePokemonApp.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<ICollection<Owner>> GetAllOwnersAsync()
        {
            return await _context.Owners.OrderBy(o => o.Id).ToListAsync();
        }

        public async Task<ICollection<Owner>> GetOwnerByPokemonIdAsync(int pokemonId)
        {
            return await _context.PokemonOwners
                .Where(po => po.Pokemon.Id == pokemonId)
                .Select(o => o.Owner).ToListAsync();
        }

        public async Task<ICollection<Pokemon>> GetPokemonByOwnerIdAsync(int ownerId)
        {
            return await _context.PokemonOwners
                            .Where(po => po.Owner.Id == ownerId)
                            .Select(p => p.Pokemon).ToListAsync();
        }

        public async Task<Owner> GetOwnerByIdAsync(int ownerId)
        {
            return await _context.Owners.Where(o => o.Id == ownerId).FirstOrDefaultAsync();

        }

        public async Task<Owner> GetOwnerByNameAsync(string lastName)
        {
            return await _context.Owners
                            .Where(c => c.LastName.Trim().ToUpper() == lastName.TrimEnd().ToUpper())
                            .FirstOrDefaultAsync();
        }

        public async Task<bool> OwnerExistsAsync(int ownerId)
        {
            return await _context.Owners.AnyAsync(o => o.Id == ownerId);
        }

        public async Task<bool> CreateOwnerAsync(Owner owner)
        {
            _context.Add(owner);

            return await SaveAsync();
        }

        public async Task<bool> UpdateOwnerAsync(Owner owner)
        {
            _context.Owners.Update(owner);

            return await SaveAsync();
        }

        public async Task<bool> DeleteOwnerAsync(Owner owner)
        {
            _context.Owners.Remove(owner);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
