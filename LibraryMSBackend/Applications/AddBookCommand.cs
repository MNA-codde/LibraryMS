using Domain;
using LibraryMSBackend.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class AddBookCommand : IRequest<BookDto>
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int TotalCopies { get; set; } = 1;
        public Guid AuthorId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? LocationId { get; set; }
    }

    public class AddBookCommandValidator : AbstractValidator<AddBookCommand>
    {
        private readonly AppDbContext _context;

        public AddBookCommandValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be 200 characters or fewer.");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .MaximumLength(20).WithMessage("ISBN must be 20 characters or fewer.")
                .MustAsync(async (isbn, ct) => !await _context.Books.AnyAsync(b => b.ISBN == isbn, ct))
                .WithMessage(x => $"A book with ISBN '{x.ISBN}' already exists.");

            RuleFor(x => x.TotalCopies)
                .GreaterThanOrEqualTo(1).WithMessage("Total copies must be at least 1.");

            RuleFor(x => x.AuthorId)
                .MustAsync(async (id, ct) => await _context.Authors.AnyAsync(a => a.Id == id, ct))
                .WithMessage("Author does not exist.");

            RuleFor(x => x.CategoryId)
                .MustAsync(async (id, ct) => await _context.Categories.AnyAsync(c => c.Id == id, ct))
                .WithMessage("Category does not exist.");

            RuleFor(x => x.LocationId)
                .MustAsync(async (id, ct) => id == null || await _context.Locations.AnyAsync(l => l.Id == id, ct))
                .WithMessage("Location does not exist.");
        }
    }

    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, BookDto>
    {
        private readonly AppDbContext _context;

        public AddBookCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookDto> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            var author = await _context.Authors.FindAsync(new object[] { request.AuthorId }, cancellationToken)
                ?? throw new InvalidOperationException("Author not found.");
            var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken)
                ?? throw new InvalidOperationException("Category not found.");

            var book = new Book(request.Title, request.ISBN, request.TotalCopies);
            book.AssignAuthor(author);
            book.AssignCategory(category);

            if (request.LocationId.HasValue)
            {
                var location = await _context.Locations.FindAsync(new object[] { request.LocationId.Value }, cancellationToken)
                    ?? throw new InvalidOperationException("Location not found.");
                book.AssignLocation(location);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync(cancellationToken);

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                AuthorName = author.FullName,
                CategoryName = category.Name,
                LocationName = book.Location?.Name,
                TotalCopies = book.TotalCopies,
                CopiesAvailable = book.CopiesAvailable
            };
        }
    }

    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? LocationName { get; set; }
        public int TotalCopies { get; set; }
        public int CopiesAvailable { get; set; }
    }
}