using LibraryWebAPI.Data;
using LibraryWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryWebAPI.Services
{
    public class BookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext appdbContext)
        {
            this._context = appdbContext;
        }
        public async Task<Book> CreateBook(BookBase book)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL AddBook({0},{1},{2},{3},{4})",
                book.Title,
                book.Author,
                book.ISBN,
                book.PublishedYear,
                book.CopiesAvailable
            );

            // retun inserted book;
            throw new NotImplementedException("not imeplemented");
        }
    }
}
