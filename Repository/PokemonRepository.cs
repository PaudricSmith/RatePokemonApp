using Microsoft.EntityFrameworkCore;
using RatePokemonApp.Data;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;

namespace RatePokemonApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Pokemon>> GetAllPokemonAsync()
        {
            return await _context.Pokemon.OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<Pokemon> GetPokemonByIdAsync(int id)
        {
            return await _context.Pokemon.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Pokemon> GetPokemonByNameAsync(string name)
        {
            return await _context.Pokemon.Where(p => p.Name == name).FirstOrDefaultAsync();
        }

        public async Task<decimal> GetPokemonAverageRatingAsync(int pokemonId)
        {
            // Check if there are any reviews for the given Pokemon ID.
            var hasReviews = await _context.Reviews.AnyAsync(r => r.Pokemon.Id == pokemonId);
            
            if (!hasReviews)
            {
                return 0;
            }

            // Calculate the average rating asynchronously and return it.
            var averageRating = await _context.Reviews
                                               .Where(r => r.Pokemon.Id == pokemonId)
                                               .AverageAsync(r => r.Rating);

            return (decimal)averageRating;
        }

        public async Task<bool> PokemonExistsAsync(int pokemonId)
        {
            return await _context.Pokemon.AnyAsync(p => p.Id == pokemonId);
        }

        public async Task<bool> CreatePokemonAsync(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };

            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.Add(pokemonCategory);

            _context.Add(pokemon);

            return await SaveAsync();
        }

        public async Task<bool> UpdatePokemonAsync(Pokemon pokemon)
        {
            _context.Pokemon.Update(pokemon);

            return await SaveAsync();
        }

        public async Task<bool> DeletePokemonAsync(Pokemon pokemon)
        {
            _context.Pokemon.Remove(pokemon);

            return await SaveAsync();
        }

        public async Task<bool> SaveAsync()
        {
            var savedChanges = await _context.SaveChangesAsync();

            return savedChanges > 0;
        }
    }
}
