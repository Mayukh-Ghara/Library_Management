using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using LibraryWebAPI.Services;

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

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books
                .FromSqlRaw("CALL SP_GetAllBooks()")
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var books = await _context.Books
                .FromSqlRaw("CALL SP_GetBookById({0})", id)
                .ToListAsync();

            var book = books.FirstOrDefault();

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookBase book)
        {
           var _book=await _bookService.CreateBook(book);
            return Ok(_book);
        }

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