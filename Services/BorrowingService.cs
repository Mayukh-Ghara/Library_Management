using LibraryWebAPI.Data;
using LibraryWebAPI.DTOs;
using LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebAPI.Services
{
    public class BorrowingService
    {
        private readonly AppDbContext _context;

        public BorrowingService(AppDbContext context)
        {
            _context = context;
        }

        // ─── 1. BORROW ───────────────────────────────────────────
        public async Task<(bool Success, string Message, BorrowingResponseDto? Data)> BorrowBookAsync(BorrowRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && u.IsActive);
                if (user == null) return (false, "User not found or inactive.", null);

                var book = await _context.Books
                    .FromSqlRaw("SELECT * FROM books WHERE ID = {0} FOR UPDATE", request.BookId)
                    .FirstOrDefaultAsync();

                if (book == null) return (false, "Book not found.", null);
                if (book.CopiesAvailable <= 0) return (false, "No copies available.", null);

                var alreadyBorrowed = await _context.Borrowings
                    .AnyAsync(b => b.UserId == request.UserId && b.BookId == request.BookId && b.Status == BorrowingStatus.Borrowed);

                if (alreadyBorrowed) return (false, "User already has this book borrowed.", null);

                // Default to 14 days if not specified
                int daysToBorrow = request.BorrowDays > 0 ? request.BorrowDays : 14;

                var borrowing = new Borrowing
                {
                    UserId = request.UserId,
                    BookId = request.BookId,
                    DueDate = DateTime.UtcNow.AddDays(daysToBorrow),
                    Status = BorrowingStatus.Borrowed
                };

                _context.Borrowings.Add(borrowing);
                book.CopiesAvailable--;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new BorrowingResponseDto
                {
                    Id = borrowing.Id,
                    BookTitle = book.Title,
                    Username = user.Username,
                    BorrowedAt = borrowing.BorrowedAt,
                    DueDate = borrowing.DueDate,
                    Status = borrowing.Status.ToString()
                };

                return (true, "Book borrowed successfully.", response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        // ─── 2. RETURN ───────────────────────────────────────────
        public async Task<(bool Success, string Message, BorrowingResponseDto? Data)> ReturnBookAsync(ReturnRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var borrowing = await _context.Borrowings
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.UserId == request.UserId && b.BookId == request.BookId && b.Status != BorrowingStatus.Returned);

                if (borrowing == null) return (false, "No active borrowing found for this user and book.", null);

                borrowing.ReturnedAt = DateTime.UtcNow;
                borrowing.Status = BorrowingStatus.Returned;

                if (borrowing.Book != null)
                {
                    borrowing.Book.CopiesAvailable++;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new BorrowingResponseDto
                {
                    Id = borrowing.Id,
                    BookTitle = borrowing.Book?.Title ?? "Unknown",
                    Username = borrowing.User?.Username ?? "Unknown",
                    BorrowedAt = borrowing.BorrowedAt,
                    DueDate = borrowing.DueDate,
                    ReturnedAt = borrowing.ReturnedAt,
                    Status = borrowing.Status.ToString()
                };

                return (true, "Book returned successfully.", response);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        // ─── 3. GET USER BORROWINGS ──────────────────────────────
        public async Task<IEnumerable<object>> GetUserBorrowingsAsync(int userId)
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .Where(b => b.UserId == userId && b.Status != BorrowingStatus.Returned)
                .ToListAsync();

            // Safe checks (?) included so dirty data doesn't crash the server
            return borrowings.Select(b => new
            {
                borrowingId = b.Id,
                bookId = b.BookId,
                title = b.Book?.Title ?? "Unknown Book",
                author = b.Book?.Author ?? "Unknown",
                borrowDate = b.BorrowedAt,
                dueDate = b.DueDate,
                status = b.Status.ToString()
            });
        }
    }
}