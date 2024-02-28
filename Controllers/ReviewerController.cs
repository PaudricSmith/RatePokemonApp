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
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReviewerDto>))]
        public async Task<IActionResult> GetAllReviewersAsync()
        {
            var reviewers = await _reviewerRepository.GetAllReviewersAsync();

            if (reviewers == null)
                return NotFound();

            var reviewerDtos = _mapper.Map<IEnumerable<ReviewerDto>>(reviewers);

            return Ok(reviewerDtos);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewerByIdAsync(int reviewerId)
        {
            var reviewer = await _reviewerRepository.GetReviewerByIdAsync(reviewerId);
            if (reviewer == null)
                return NotFound();

            var reviewerDto = _mapper.Map<ReviewerDto>(reviewer);

            return Ok(reviewerDto);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReviewDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReviewsByReviewerAsync(int reviewerId)
        {
            var reviews = await _reviewerRepository.GetReviewsByReviewerAsync(reviewerId);
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
        public async Task<IActionResult> CreateReviewerAsync([FromBody] ReviewerDto reviewerCreate)
        {
            // Check for null DTO
            if (reviewerCreate == null)
                return BadRequest("Reviewer data is required.");

            // Check if the reviewer already exists
            var reviewer = await _reviewerRepository.GetReviewerByNameAsync(reviewerCreate.LastName);
            if (reviewer != null)
            {
                ModelState.AddModelError("Title", "Reviewer already exists!");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            // Map DTO to entity
            var reviewerEntity = _mapper.Map<Reviewer>(reviewerCreate);

            // Attempt to create the review
            var createResult = await _reviewerRepository.CreateReviewerAsync(reviewerEntity);
            if (!createResult)
            {
                ModelState.AddModelError("", "Something went wrong while saving the reviewer.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Return a 201 Created response with the created review
            return CreatedAtAction("GetReviewerById", new { reviewerId = reviewerEntity.Id }, reviewerEntity);
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReviewerAsync(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
        {
            // Check for null DTO
            if (updatedReviewer == null)
                return BadRequest("Reviewer data is required.");

            if (reviewerId != updatedReviewer.Id)
                return BadRequest("Reviewer Ids do not match.");

            // Check if the reviewer already exists
            var existingReviewer = await _reviewerRepository.GetReviewerByIdAsync(reviewerId);
            if (existingReviewer == null)
                return NotFound($"Reviewer with Id {reviewerId} not found.");

            // Map DTO to entity
            _mapper.Map(updatedReviewer, existingReviewer);

            // Attempt to update the reviewer
            var updateResult = await _reviewerRepository.UpdateReviewerAsync(existingReviewer);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Something went wrong while updating the reviewer.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            // Indicating successful update without returning the resource
            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReviewerAsync(int reviewerId)
        {
            // Check if the reviewer already exists
            var reviewerToDelete = await _reviewerRepository.GetReviewerByIdAsync(reviewerId);
            if (reviewerToDelete == null)
                return NotFound($"Reviewer with Id {reviewerId} not found.");

            // Attempt to delete review
            var deleteResult = await _reviewerRepository.DeleteReviewerAsync(reviewerToDelete);
            if (!deleteResult)
            {
                ModelState.AddModelError("", "Something went wrong while deleting the reviewer.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
