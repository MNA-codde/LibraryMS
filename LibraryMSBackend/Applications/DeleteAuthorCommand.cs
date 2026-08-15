using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class DeleteAuthorCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteAuthorCommandHandler : IRequestHandler<DeleteAuthorCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteAuthorCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await _context.Authors.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new InvalidOperationException("Author not found.");

            var hasBooks = await _context.Books.AnyAsync(b => b.AuthorId == request.Id, cancellationToken);
            if (hasBooks)
                throw new InvalidOperationException("Cannot delete an author with existing books. Reassign or delete those books first.");

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}