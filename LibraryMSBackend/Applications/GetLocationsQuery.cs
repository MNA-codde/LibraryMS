using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class GetLocationsQuery : IRequest<List<LocationDto>> { }

    public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, List<LocationDto>>
    {
        private readonly AppDbContext _context;

        public GetLocationsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Locations
                .AsNoTracking()
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                })
                .ToListAsync(cancellationToken);
        }
    }
}