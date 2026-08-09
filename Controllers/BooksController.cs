using LibraryWebAPI.Data;
using LibraryWebAPI.DTOs;
using LibraryWebAPI.Models;
using LibraryWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BookService _bookService;

        public BooksController(AppDbContext context, BookService bookService)
        {
            _context = context;
            this._bookService = bookService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] string search = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6)
        {
            search = search?.Trim() ?? "";

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 6;

            var books = await _context.Books
                .FromSqlRaw("CALL SP_GetAllBooks({0}, {1}, {2})", search, page, pageSize)
                .ToListAsync();

            // ✅ ToListAsync instead of FirstOrDefaultAsync
            var countList = await _context.BookCounts
                .FromSqlRaw("CALL SP_GetBooksCount({0})", search)
                .ToListAsync();

            int totalCount = countList.FirstOrDefault()?.Count ?? 0;

            var result = new PagedResult<Book>
            {
                Data = books,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }

        [Authorize(Roles = "Admin, User, admin, user")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            List<Book> books = await _context.Books
                .FromSqlRaw("CALL SP_GetBookById({0})", id)
                .ToListAsync();

            var book = books.FirstOrDefault();

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookBase book)
        {
            var _book = await _bookService.CreateBook(book);
            return Ok(_book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book book)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL UpdateBook({0},{1},{2},{3},{4},{5})",
                id,
                book.Title,
                book.Author,
                book.ISBN,
                book.PublishedYear,
                book.CopiesAvailable
            );

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL SP_DeleteBook({0})",
                id
            );

            return NoContent();
        }
    }
}