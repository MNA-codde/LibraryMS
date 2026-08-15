using LibraryMSBackend.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken)
                ?? throw new InvalidOperationException("Category not found.");

            var hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == request.Id, cancellationToken);
            if (hasBooks)
                throw new InvalidOperationException("Cannot delete a category with existing books. Reassign or delete those books first.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}