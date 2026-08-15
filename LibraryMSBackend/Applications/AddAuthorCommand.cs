using Domain;
using LibraryMSBackend.Infrastructure;
using FluentValidation;
using MediatR;

namespace LibraryMSBackend.Applications
{
    public class AddAuthorCommand : IRequest<AuthorDto>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class AddAuthorCommandValidator : AbstractValidator<AddAuthorCommand>
    {
        public AddAuthorCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        }
    }

    public class AddAuthorCommandHandler : IRequestHandler<AddAuthorCommand, AuthorDto>
    {
        private readonly AppDbContext _context;

        public AddAuthorCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorDto> Handle(AddAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = new Author(request.FirstName, request.LastName);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthorDto { Id = author.Id, FullName = author.FullName };
        }
    }

    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}