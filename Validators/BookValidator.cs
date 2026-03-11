using FluentValidation;
using LibraryWebAPI.Models;

namespace LibraryWebAPI.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(book => book.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200);

            RuleFor(book => book.Author)
                .NotEmpty().WithMessage("Author is required")
                .MaximumLength(150);

            RuleFor(book => book.ISBN)
                .NotEmpty()
                .Length(10, 20);

            RuleFor(book => book.PublishedYear)
                .InclusiveBetween(1000, DateTime.Now.Year);

            RuleFor(book => book.CopiesAvailable)
                .GreaterThanOrEqualTo(0);
        }
    }
}