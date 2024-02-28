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
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, 
            IPokemonRepository pokemonRepository,
            IReviewerRepository reviewerRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewDto>))]
        public async Task<IActionResult> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();

            if (reviews == null)
                return NotFound();

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return Ok(reviewDtos);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewByIdAsync(int reviewId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                return NotFound();

            var reviewDto = _mapper.Map<ReviewDto>(review);

            return Ok(reviewDto);
        }

        [HttpGet("pokemon/{pokemonId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewsByPokemonIdAsync(int pokemonId)
        {
            var reviews = await _reviewRepository.GetReviewsByPokemonIdAsync(pokemonId);
            if (reviews == null)
                return NotFound();

            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            return Ok(reviewDtos);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReviewAsync([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDto reviewCreate)
        {
            // Check for null DTO
            if (reviewCreate == null)
                return BadRequest("Review data is required.");

            // Check if the review already exists
            var review = await _reviewRepository.GetReviewByNameAsync(reviewCreate.Title);
            if (review != null)
            {
                ModelState.AddModelError("Title", "Review already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var reviewEntity = _mapper.Map<Review>(reviewCreate);
            
            reviewEntity.Pokemon = await _pokemonRepository.GetPokemonByIdAsync(pokemonId);
            reviewEntity.Reviewer = await _reviewerRepository.GetReviewerByIdAsync(reviewerId);


            // Attempt to create the review
            var createResult = await _reviewRepository.CreateReviewAsync(reviewEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the review.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created review
            return CreatedAtAction("GetReviewById", new { reviewId = reviewEntity.Id }, reviewEntity);
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReviewAsync(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            // Check for null DTO
            if (updatedReview == null)
                return BadRequest("Review data is required.");

            if (reviewId != updatedReview.Id)
                return BadRequest("Review Ids do not match.");

            // Check if the review already exists
            var existingReview = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (existingReview == null)
                return NotFound($"Review with Id {reviewId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedReview, existingReview);

            // Attempt to update the review
            var updateResult = await _reviewRepository.UpdateReviewAsync(existingReview);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the review.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReviewAsync(int reviewId)
        {
            // Check if the review already exists
            var reviewToDelete = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (reviewToDelete == null)
                return NotFound($"Review with Id {reviewId} not found.");

            // Attempt to delete review
            var deleteResult = await _reviewRepository.DeleteReviewAsync(reviewToDelete);
            if (!deleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the review.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
