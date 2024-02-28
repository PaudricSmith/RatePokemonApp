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
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OwnerDto>))]
        public async Task<IActionResult> GetAllOwnersAsync()
        {
            var owners = await _ownerRepository.GetAllOwnersAsync();

            if (owners == null)
                return NotFound();

            var ownerDtos = _mapper.Map<IEnumerable<OwnerDto>>(owners);

            return Ok(ownerDtos);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OwnerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnerByIdAsync(int ownerId)
        {
            var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (owner == null)
                return NotFound();

            var ownerDto = _mapper.Map<OwnerDto>(owner);

            return Ok(ownerDto);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OwnerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPokemonByOwnerIdAsync(int ownerId)
        {
            var pokemon = await _ownerRepository.GetPokemonByOwnerIdAsync(ownerId);
            if (pokemon == null)
                return NotFound();

            var pokemonDtos = _mapper.Map<IEnumerable<PokemonDto>>(pokemon);

            return Ok(pokemonDtos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOwnerAsync([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            // Check for null DTO
            if (ownerCreate == null)
                return BadRequest("Owner data is required.");

            // Check if the owner already exists
            var owner = await _ownerRepository.GetOwnerByNameAsync(ownerCreate.LastName);
            if (owner != null)
            {
                ModelState.AddModelError("LastName", "Owner already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var ownerEntity = _mapper.Map<Owner>(ownerCreate);

            ownerEntity.Country = await _countryRepository.GetCountryByIdAsync(countryId);

            // Attempt to create the country
            var createResult = await _ownerRepository.CreateOwnerAsync(ownerEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the owner.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created owner
            return CreatedAtAction("GetOwnerById", new { ownerId = ownerEntity.Id }, ownerEntity);
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOwnerAsync(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            // Check for null DTO
            if (updatedOwner == null)
                return BadRequest("Owner data is required.");

            if (ownerId != updatedOwner.Id)
                return BadRequest("Owner Ids do not match.");

            // Check if the owner already exists
            var existingOwner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (existingOwner == null)
                return NotFound($"Owner with Id {ownerId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedOwner, existingOwner);

            // Attempt to update the owner
            var updateResult = await _ownerRepository.UpdateOwnerAsync(existingOwner);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the owner.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOwnerAsync(int ownerId)
        {
            // Check if the country already exists
            var ownerToDelete = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            if (ownerToDelete == null)
                return NotFound($"Owner with Id {ownerId} not found.");

            // Attempt to delete the owner
            var deleteResult = await _ownerRepository.DeleteOwnerAsync(ownerToDelete);
            if (!deleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the owner.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
