using Domain;
using LibraryMSBackend.Infrastructure;
using FluentValidation;
using MediatR;

namespace LibraryMSBackend.Applications
{
    public class AddLocationCommand : IRequest<LocationDto>
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AddLocationCommandValidator : AbstractValidator<AddLocationCommand>
    {
        public AddLocationCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
            RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        }
    }

    public class AddLocationCommandHandler : IRequestHandler<AddLocationCommand, LocationDto>
    {
        private readonly AppDbContext _context;

        public AddLocationCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LocationDto> Handle(AddLocationCommand request, CancellationToken cancellationToken)
        {
            var location = new Location(request.Name, request.Latitude, request.Longitude);
            _context.Locations.Add(location);
            await _context.SaveChangesAsync(cancellationToken);

            return new LocationDto
            {
                Id = location.Id,
                Name = location.Name,
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
        }
    }

    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}