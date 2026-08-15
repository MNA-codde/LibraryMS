using Domain;
using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetBooksQuery : IRequest<List<BookDto>> { }

    public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, List<BookDto>>
    {
        private readonly AppDbContext _context;

        public GetBooksQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
        {
            return await _context.Books
            .AsNoTracking()
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Location)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorName = b.Author.FullName,
                    CategoryName = b.Category != null ? b.Category.Name : string.Empty,
                    LocationName = b.Location != null ? b.Location.Name : string.Empty,
                    TotalCopies = b.TotalCopies,
                    CopiesAvailable = b.CopiesAvailable
                })
                .ToListAsync(cancellationToken);
        }
    }
}