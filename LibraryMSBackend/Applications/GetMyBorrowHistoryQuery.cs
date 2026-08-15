using Domain;
using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetMyBorrowHistoryQuery : IRequest<List<BorrowRecordDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetMyBorrowHistoryQueryHandler : IRequestHandler<GetMyBorrowHistoryQuery, List<BorrowRecordDto>>
    {
        private readonly AppDbContext _context;

        public GetMyBorrowHistoryQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BorrowRecordDto>> Handle(GetMyBorrowHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _context.BorrowRecords
            .AsNoTracking()
                .Include(br => br.Book)
                .Where(br => br.UserId == request.UserId)
                    .OrderByDescending(br => br.BorrowDate)

                .Select(br => new BorrowRecordDto
                {
                    Id = br.Id,
                    BookTitle = br.Book.Title,
                    BorrowDate = br.BorrowDate,
                    DueDate = br.DueDate,
                    Status = br.Status.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}