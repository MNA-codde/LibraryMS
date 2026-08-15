using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetAuthorsQuery : IRequest<List<AuthorDto>> { }

    public class GetAuthorsQueryHandler : IRequestHandler<GetAuthorsQuery, List<AuthorDto>>
    {
        private readonly AppDbContext _context;

        public GetAuthorsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuthorDto>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Authors
            .AsNoTracking()
                .Select(a => new AuthorDto { Id = a.Id, FullName = a.FullName })
                .ToListAsync(cancellationToken);
        }
    }
}