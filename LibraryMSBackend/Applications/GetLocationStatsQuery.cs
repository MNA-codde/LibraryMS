using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetLocationStatsQuery : IRequest<List<LocationStatsDto>> { }

    public class GetLocationStatsQueryHandler : IRequestHandler<GetLocationStatsQuery, List<LocationStatsDto>>
    {
        private readonly AppDbContext _context;

        public GetLocationStatsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocationStatsDto>> Handle(GetLocationStatsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Locations
                .AsNoTracking()
                .Select(l => new LocationStatsDto
                {
                    LocationId = l.Id,
                    Name = l.Name,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude,
                    BookCount = l.Books.Count()
                })
                .OrderByDescending(x => x.BookCount)
                .ToListAsync(cancellationToken);
        }
    }

    public class LocationStatsDto
    {
        public Guid LocationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BookCount { get; set; }
    }
}