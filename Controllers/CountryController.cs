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
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CountryDto>))]
        public async Task<IActionResult> GetAllCountriesAsync()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();

            if (countries == null)
                return NotFound();

            var countryDtos = _mapper.Map<IEnumerable<CountryDto>>(countries);

            return Ok(countryDtos);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryByIdAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryByIdAsync(countryId);
            if (country == null)
                return NotFound();

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CountryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCountryByOwnerIdAsync(int ownerId)
        {
            var country = await _countryRepository.GetCountryByOwnerIdAsync(ownerId);
            if (country == null)
                return NotFound();

            var countryDto = _mapper.Map<CountryDto>(country);

            return Ok(countryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCountryAsync([FromBody] CountryDto countryCreate)
        {
            // Check for null DTO
            if (countryCreate == null)
                return BadRequest("Country data is required.");

            // Check if the country already exists
            var country = await _countryRepository.GetCountryByNameAsync(countryCreate.Name);
            if (country != null)
            {
                ModelState.AddModelError("Name", "Country already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var countryEntity = _mapper.Map<Country>(countryCreate);

            // Attempt to create the country
            var createResult = await _countryRepository.CreateCountryAsync(countryEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the country.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created country
            return CreatedAtAction("GetCountryById", new { countryId = countryEntity.Id }, countryEntity);
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountryAsync(int countryId, [FromBody] CountryDto updatedCountry)
        {
            // Check for null DTO
            if (updatedCountry == null)
                return BadRequest("Country data is required.");

            if (countryId != updatedCountry.Id)
                return BadRequest("Country Ids do not match.");

            // Check if the country already exists
            var existingCountry = await _countryRepository.GetCountryByIdAsync(countryId);
            if (existingCountry == null)
                return NotFound($"Country with Id {countryId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedCountry, existingCountry);

            // Attempt to update the country
            var updateResult = await _countryRepository.UpdateCountryAsync(existingCountry);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the country.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCountryAsync(int countryId)
        {
            // Check if the country already exists
            var countryToDelete = await _countryRepository.GetCountryByIdAsync(countryId);
            if (countryToDelete == null)
                return NotFound($"Country with Id {countryId} not found.");

            // Attempt to delete the country
            var deleteResult = await _countryRepository.DeleteCountryAsync(countryToDelete);
            if (!deleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the country.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
