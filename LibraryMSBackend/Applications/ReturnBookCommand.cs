using Domain;
using LibraryMSBackend.Infrastructure;
using LibraryMSBackend.Infrastructure.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Applications
{
    public class ReturnBookCommand : IRequest<BorrowRecordDto>
    {
        public Guid BorrowRecordId { get; set; }
        public Guid UserId { get; set; }
    }

    public class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
    {
        private readonly AppDbContext _context;
        

        public ReturnBookCommandValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(x => x.BorrowRecordId)
                .MustAsync(async (id, ct) => await _context.BorrowRecords.AnyAsync(r => r.Id == id, ct))
                .WithMessage("Borrow record not found.");

            RuleFor(x => x)
                .MustAsync(async (cmd, ct) =>
                {
                    var record = await _context.BorrowRecords
                        .FirstOrDefaultAsync(r => r.Id == cmd.BorrowRecordId, ct);
                    return record != null && record.UserId == cmd.UserId;
                })
                .WithMessage("You can only return your own borrowed books.");
        }
    }

    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowRecordDto>
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ReturnBookCommandHandler(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<BorrowRecordDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _context.Books
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.BorrowRecords.Any(r => r.Id == request.BorrowRecordId), cancellationToken)
                ?? throw new InvalidOperationException("Borrow record not found.");

            var record = book.BorrowRecords.First(r => r.Id == request.BorrowRecordId);

            book.Return(record);

            await _context.SaveChangesAsync(cancellationToken);

             await _hubContext.Clients.Group("Admins").SendAsync(
                "BookReturned",
                $"\"{book.Title}\" was just returned.",
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
}