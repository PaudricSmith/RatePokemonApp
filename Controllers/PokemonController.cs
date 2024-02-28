using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatePokemonApp.Dto;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;
using RatePokemonApp.Repository;

namespace RatePokemonApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PokemonDto>))]
        public async Task<IActionResult> GetAllPokemonAsync()
        {
            var pokemons = await _pokemonRepository.GetAllPokemonAsync();
            if (pokemons == null)
                return NotFound();

            var pokemonDtos = _mapper.Map<IEnumerable<PokemonDto>>(pokemons);

            return Ok(pokemonDtos);
        }

        [HttpGet("{pokemonId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PokemonDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPokemonByIdAsync(int pokemonId)
        {
            var pokemon = await _pokemonRepository.GetPokemonByIdAsync(pokemonId);
            if (pokemon == null)
                return NotFound();

            var pokemonDto = _mapper.Map<PokemonDto>(pokemon);

            return Ok(pokemonDto);
        }

        [HttpGet("{pokemonId}/rating")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPokemonAverageRatingAsync(int pokemonId)
        {
            var pokemonExists = await _pokemonRepository.PokemonExistsAsync(pokemonId);
            if (!pokemonExists)
                return NotFound();
                
            var averageRating = await _pokemonRepository.GetPokemonAverageRatingAsync(pokemonId);

            return Ok(averageRating);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePokemonAsync([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
        {
            // Check for null DTO
            if (pokemonCreate == null)
                return BadRequest("Pokemon data is required.");

            // Check if the pokemon already exists
            var pokemon = await _pokemonRepository.GetPokemonByNameAsync(pokemonCreate.Name);
            if (pokemon != null)
            {
                ModelState.AddModelError("Name", "Pokemon already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var pokemonEntity = _mapper.Map<Pokemon>(pokemonCreate);

            // Attempt to create the pokemon
            var createResult = await _pokemonRepository.CreatePokemonAsync(ownerId, categoryId, pokemonEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the pokemon.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created pokemon
            return CreatedAtAction("GetPokemonById", new { pokemonId = pokemonEntity }, pokemonEntity);
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePokemonAsync(int pokemonId, [FromBody] PokemonDto updatedPokemon)
        {
            // Check for null DTO
            if (updatedPokemon == null)
                return BadRequest("Pokemon data is required.");

            if (pokemonId != updatedPokemon.Id)
                return BadRequest("Pokemon Ids do not match.");

            // Check if the pokemon already exists
            var existingPokemon = await _pokemonRepository.GetPokemonByIdAsync(pokemonId);
            if (existingPokemon == null)
                return NotFound($"Pokemon with Id {pokemonId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedPokemon, existingPokemon);

            // Attempt to update the pokemon
            var updateResult = await _pokemonRepository.UpdatePokemonAsync(existingPokemon);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the pokemon.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePokemonAsync(int pokemonId)
        {
            // Check if the pokemon already exists
            var pokemonToDelete = await _pokemonRepository.GetPokemonByIdAsync(pokemonId);
            if (pokemonToDelete == null)
                return NotFound($"Pokemon with Id {pokemonId} not found.");

            // Check if the reviews already exist
            var reviewsToDelete = await _reviewRepository.GetReviewsByPokemonIdAsync(pokemonId);
            if (reviewsToDelete == null)
                return NotFound($"Reviews with Id {pokemonId} not found.");

            // Attempt to delete the list of reviews
            var reviewsDeleteResult = await _reviewRepository.DeleteReviewsAsync(reviewsToDelete.ToList());
            if (!reviewsDeleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the reviews.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Attempt to delete the pokemon
            var pokemonDeleteResult = await _pokemonRepository.DeletePokemonAsync(pokemonToDelete);
            if (!pokemonDeleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the pokemon.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
