using System.Security.Claims;
using LibraryWebAPI.DTOs;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Tags("Reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // POST api/reviews/book/3
        [HttpPost("book/{bookId}")]
        public async Task<IActionResult> CreateReview(int bookId, [FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (success, message, data) = await _reviewService.CreateReviewAsync(userId, bookId, dto);

            if (!success) return BadRequest(new { message });
            return Ok(new { message, data });
        }

        // PUT api/reviews/5
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (success, message, data) = await _reviewService.UpdateReviewAsync(reviewId, userId, dto);

            if (!success) return BadRequest(new { message });
            return Ok(new { message, data });
        }

        // DELETE api/reviews/5
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;

            var (success, message) = await _reviewService.DeleteReviewAsync(reviewId, userId, userRole);

            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        // GET api/reviews/book/3  — public, no auth needed
        [HttpGet("book/{bookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookReviews(int bookId)
        {
            var result = await _reviewService.GetBookReviewsAsync(bookId);

            if (result == null) return NotFound(new { message = "Book not found." });
            return Ok(result);
        }

        // GET api/reviews/my-reviews
        [HttpGet("my-reviews")]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var reviews = await _reviewService.GetUserReviewsAsync(userId);
            return Ok(reviews);
        }
    }
}
