using LibraryMSBackend.Infrastructure;
using LibraryMSBackend.Infrastructure.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSBackend.Infrastructure.Jobs
{
    public class OverdueCheckJob
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OverdueCheckJob(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task RunAsync()
        {
            var overdueCount = await _context.BorrowRecords
                .AsNoTracking()
                .Where(r => r.ReturnDate == null && r.DueDate < DateTime.UtcNow)
                .CountAsync();

            if (overdueCount > 0)
            {
                await _hubContext.Clients.Group("Admins").SendAsync(
                    "OverdueSummary",
                    $"{overdueCount} book(s) are currently overdue.");
            }
        }
    }
}