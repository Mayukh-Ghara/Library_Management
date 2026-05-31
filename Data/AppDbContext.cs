using Microsoft.EntityFrameworkCore;
using LibraryWebAPI.Models;

namespace LibraryWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<Review> Reviews { get; set; } // 🆕

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── User ──────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();      // no duplicate emails
                entity.HasIndex(u => u.Username).IsUnique();   // no duplicate usernames
                entity.Property(u => u.Role).HasDefaultValue("user");
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            // ─── Borrowing ─────────────────────────────────────
            modelBuilder.Entity<Borrowing>(entity =>
            {
                // Store enum as string instead of int
                entity.Property(b => b.Status)
                      .HasConversion<string>();

                // Borrowing → User relationship
                entity.HasOne(b => b.User)
                      .WithMany(u => u.Borrowings)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // don't delete borrowings if user is deleted

                // Borrowing → Book relationship
                entity.HasOne(b => b.Book)
                      .WithMany(bk => bk.Borrowings)
                      .HasForeignKey(b => b.BookId)
                      .OnDelete(DeleteBehavior.Restrict); // don't delete borrowings if book is deleted
            });

            // ─── Review ────────────────────────────────────────
            modelBuilder.Entity<Review>(entity =>
            {
                // One user can only review a book once
                entity.HasIndex(r => new { r.UserId, r.BookId }).IsUnique();

                // Rating must be between 1 and 5 
                // ⚠️ UPDATED: Changed to lowercase 'rating' to match your snake_case database schema
                entity.HasCheckConstraint("ck_review_rating", "rating >= 1 AND rating <= 5");

                // Review → User relationship
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reviews)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // delete reviews if user is deleted

                // Review → Book relationship
                entity.HasOne(r => r.Book)
                      .WithMany(b => b.Reviews)
                      .HasForeignKey(r => r.BookId)
                      .OnDelete(DeleteBehavior.Cascade); // delete reviews if book is deleted
            });
        }
    }
}