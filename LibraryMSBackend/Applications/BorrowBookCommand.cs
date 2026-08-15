using Domain;
using LibraryMSBackend.Infrastructure;
using LibraryMSBackend.Infrastructure.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;


namespace LibraryMSBackend.Applications
{
    public class BorrowBookCommand : IRequest<BorrowRecordDto>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
    {
        private readonly AppDbContext _context;

        public BorrowBookCommandValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(x => x.BookId)
                .MustAsync(async (id, ct) => await _context.Books.AnyAsync(b => b.Id == id, ct))
                .WithMessage("Book does not exist.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");

            RuleFor(x => x.UserId)
                .MustAsync(async (userId, ct) =>
                {
                    var activeBorrows = await _context.BorrowRecords
                        .CountAsync(r => r.UserId == userId && r.ReturnDate == null, ct);
                    return activeBorrows < 2;
                })
                .WithMessage("You already have 2 books borrowed. Return one before borrowing another.");
        }
    }

    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowRecordDto>
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public BorrowBookCommandHandler(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;        
        }

        public async Task<BorrowRecordDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Books.FindAsync(new object[] { request.BookId }, cancellationToken)
                ?? throw new InvalidOperationException("Book not found.");

            var record = book.Borrow(request.UserId, request.DueDate);

            _context.BorrowRecords.Add(record);
            await _context.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group("Admins").SendAsync(
                "BookBorrowed",
                $"\"{book.Title}\" was just borrowed.",
                cancellationToken: cancellationToken);

            

            return new BorrowRecordDto
            {
                Id = record.Id,
                BookTitle = book.Title,
                BorrowDate = record.BorrowDate,
                DueDate = record.DueDate,
                Status = record.Status.ToString()
            };
        }
    }

    public class BorrowRecordDto
    {
        public Guid Id { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}