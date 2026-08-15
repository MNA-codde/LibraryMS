using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class User : IdentityUser<Guid>
    {
        private readonly List<BorrowRecord> _borrowRecords = new();

        public string FullName { get; set; } = string.Empty;

        public IReadOnlyCollection<BorrowRecord> BorrowRecords => _borrowRecords.AsReadOnly();
    }
}