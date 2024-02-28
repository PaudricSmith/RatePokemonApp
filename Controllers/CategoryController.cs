using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RatePokemonApp.Dto;
using RatePokemonApp.Interfaces;
using RatePokemonApp.Models;

namespace RatePokemonApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDto>))]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();

            if (categories == null)
                return NotFound();

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return Ok(categoryDtos);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                return NotFound();

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Ok(categoryDto);
        }

        [HttpGet("pokemon{categoryId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPokemonByCategoryIdAsync(int categoryId)
        {
            var pokemons = await _categoryRepository.GetPokemonByCategoryIdAsync(categoryId);
            if (pokemons == null)
                return NotFound();

            var pokemonDtos = _mapper.Map<IEnumerable<PokemonDto>>(pokemons);

            return Ok(pokemonDtos);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryDto categoryCreate)
        {
            // Check for null DTO
            if (categoryCreate == null)
                return BadRequest("Category data is required.");

            // Check if the category already exists
            var category = await _categoryRepository.GetCategoryByNameAsync(categoryCreate.Name);
            if (category != null)
            {
                ModelState.AddModelError("Name", "Category already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var categoryEntity = _mapper.Map<Category>(categoryCreate);

            // Attempt to create the category
            var createResult = await _categoryRepository.CreateCategoryAsync(categoryEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the category.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created category
            return CreatedAtAction("GetCategoryById", new { categoryId = categoryEntity.Id }, categoryEntity);
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId, [FromBody] CategoryDto updatedCategory) 
        {
            // Check for null DTO
            if (updatedCategory == null)
                return BadRequest("Category data is required.");

            if (categoryId != updatedCategory.Id)
                return BadRequest("Category Ids do not match.");

            // Check if the category already exists
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (existingCategory == null)
                return NotFound($"Category with Id {categoryId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedCategory, existingCategory);

            // Attempt to update the category
            var updateResult = await _categoryRepository.UpdateCategoryAsync(existingCategory);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the category.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            // Check if the category already exists
            var categoryToDelete = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (categoryToDelete == null)
                return NotFound($"Category with Id {categoryId} not found.");

            // Attempt to delete the category
            var deleteResult = await _categoryRepository.DeleteCategoryAsync(categoryToDelete);
            if (!deleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the category.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
