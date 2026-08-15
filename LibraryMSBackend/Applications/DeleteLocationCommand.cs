using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class DeleteLocationCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteLocationCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var location = await _context.Locations.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new InvalidOperationException("Location not found.");

            var hasBooks = await _context.Books.AnyAsync(b => b.LocationId == request.Id, cancellationToken);
            if (hasBooks)
                throw new InvalidOperationException("Cannot delete a location with existing books. Reassign or delete those books first.");

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}