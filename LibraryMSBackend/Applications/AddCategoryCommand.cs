using Domain;
using LibraryMSBackend.Infrastructure;
using FluentValidation;
using MediatR;

namespace LibraryMSBackend.Applications
{
    public class AddCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        public AddCategoryCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }

    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, CategoryDto>
    {
        private readonly AppDbContext _context;

        public AddCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category(request.Name);
            if (!string.IsNullOrWhiteSpace(request.Description))
                category.SetDescription(request.Description);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return new CategoryDto { Id = category.Id, Name = category.Name };
        }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}