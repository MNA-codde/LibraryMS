using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class DeleteBookCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteBookCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new InvalidOperationException("Book not found.");

            var hasActiveBorrows = await _context.BorrowRecords
                .AnyAsync(r => r.BookId == request.Id && r.ReturnDate == null, cancellationToken);
            if (hasActiveBorrows)
                throw new InvalidOperationException("Cannot delete a book that is currently borrowed.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}