using System;
using System.ComponentModel.DataAnnotations.Schema; // You need this using statement

namespace LibraryWebAPI.Models
{
    [Table("borrowings")] // Maps the class to the "borrowings" table
    public class Borrowing
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("book_id")]
        public int BookId { get; set; }

        [Column("borrowed_at")]
        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("returned_at")]
        public DateTime? ReturnedAt { get; set; }

        [Column("status")]
        public BorrowingStatus Status { get; set; } = BorrowingStatus.Borrowed;

        // Navigation properties do NOT get mapped to database columns, 
        // so they don't need [Column] attributes.
        public User? User { get; set; }
        public Book? Book { get; set; }
    }

    public enum BorrowingStatus
    {
        Borrowed,
        Returned,
        Overdue
    }
}