using System;

namespace Domain
{
	public enum BorrowStatus
	{
		Borrowed,
		Returned,
		Overdue,
		Lost
	}

	public class BorrowRecord
	{
		protected BorrowRecord() { }

		public BorrowRecord(Guid bookId, Guid userId, DateTime borrowDate, DateTime dueDate)
		{
			Id = Guid.NewGuid();
			BookId = bookId;
			UserId = userId;
			BorrowDate = borrowDate;
			DueDate = dueDate;
		}

		public Guid Id { get; private set; }

		public Guid BookId { get; private set; }
		public Book Book { get; private set; } = null!;

		public Guid UserId { get; private set; }
		public User User { get; private set; } = null!;

		public DateTime BorrowDate { get; private set; }
		public DateTime DueDate { get; private set; }
		public DateTime? ReturnDate { get; private set; }

		public BorrowStatus Status
		{
			get
			{
				if (ReturnDate.HasValue) return BorrowStatus.Returned;
				if (DateTime.UtcNow > DueDate) return BorrowStatus.Overdue;
				return BorrowStatus.Borrowed;
			}
		}

		public bool IsOverdue => Status == BorrowStatus.Overdue;

		public void MarkReturned()
		{
			if (ReturnDate.HasValue) return;
			ReturnDate = DateTime.UtcNow;
		}
	}
}