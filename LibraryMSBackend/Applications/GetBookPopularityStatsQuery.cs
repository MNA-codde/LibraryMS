using Domain;
using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetBookPopularityStatsQuery : IRequest<List<BookPopularityDto>>
    {
    }

    public class GetBookPopularityStatsQueryHandler
        : IRequestHandler<GetBookPopularityStatsQuery, List<BookPopularityDto>>
    {
        private readonly AppDbContext _context;

        public GetBookPopularityStatsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookPopularityDto>> Handle(
            GetBookPopularityStatsQuery request, CancellationToken cancellationToken)
        {
            return await _context.BorrowRecords
                .AsNoTracking()
                .GroupBy(r => new { r.BookId, r.Book.Title })
                .Select(g => new BookPopularityDto
                {
                    BookId = g.Key.BookId,
                    Title = g.Key.Title,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .ToListAsync(cancellationToken);
        }
    }

    public class BookPopularityDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int BorrowCount { get; set; }
    }
}