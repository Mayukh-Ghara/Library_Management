using LibraryWebAPI.Data;
using LibraryWebAPI.DTOs;
using LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebAPI.Services
{
    // Services/ReviewService.cs
    public class ReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        // ─── CREATE ────────────────────────────────────────────
        public async Task<(bool Success, string Message, ReviewResponseDto? Data)>
            CreateReviewAsync(int userId, int bookId, CreateReviewDto dto)
        {
            // Check book exists
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return (false, "Book not found.", null);

            // Check user has actually borrowed this book
            var hasBorrowed = await _context.Borrowings
                .AnyAsync(b => b.UserId == userId
                            && b.BookId == bookId
                            && b.Status == BorrowingStatus.Returned);

            if (!hasBorrowed)
                return (false, "You can only review books you have borrowed and returned.", null);

            // Check user hasn't already reviewed this book
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.BookId == bookId);

            if (alreadyReviewed)
                return (false, "You have already reviewed this book.", null);

            var review = new Review
            {
                UserId = userId,
                BookId = bookId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Reload with user and book info
            await _context.Entry(review).Reference(r => r.User).LoadAsync();

            return (true, "Review submitted successfully.", MapToDto(review, book.Title));
        }

        // ─── UPDATE ────────────────────────────────────────────
        public async Task<(bool Success, string Message, ReviewResponseDto? Data)>
            UpdateReviewAsync(int reviewId, int userId, UpdateReviewDto dto)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return (false, "Review not found.", null);

            // Only the owner can update their review
            if (review.UserId != userId)
                return (false, "You can only update your own reviews.", null);

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (true, "Review updated successfully.", MapToDto(review, review.Book.Title));
        }

        // ─── DELETE ────────────────────────────────────────────
        public async Task<(bool Success, string Message)>
            DeleteReviewAsync(int reviewId, int userId, string userRole)
        {
            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null)
                return (false, "Review not found.");

            // Owner OR admin can delete
            if (review.UserId != userId && userRole != "admin")
                return (false, "You can only delete your own reviews.");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return (true, "Review deleted successfully.");
        }

        // ─── GET ALL REVIEWS FOR A BOOK ────────────────────────
        public async Task<BookReviewSummaryDto?> GetBookReviewsAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return null;

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return new BookReviewSummaryDto
            {
                BookTitle = book.Title,
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0,
                TotalReviews = reviews.Count,
                Reviews = reviews.Select(r => MapToDto(r, book.Title)).ToList()
            };
        }

        // ─── GET MY REVIEWS ────────────────────────────────────
        public async Task<List<ReviewResponseDto>> GetUserReviewsAsync(int userId)
        {
            return await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToDto(r, r.Book.Title))
                .ToListAsync();
        }

        // ─── HELPER ────────────────────────────────────────────
        private static ReviewResponseDto MapToDto(Review review, string bookTitle) => new()
        {
            Id = review.Id,
            Username = review.User.Username,
            BookTitle = bookTitle,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}
