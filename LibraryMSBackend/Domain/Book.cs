using System;
using System.Collections.Generic;

namespace Domain
{
	public class Book
	{
		private readonly List<BorrowRecord> _borrowRecords = new();

		protected Book() { }

		public Book(string title, string isbn, int totalCopies = 1)
		{
			Id = Guid.NewGuid();
			Title = title ?? throw new ArgumentNullException(nameof(title));
			ISBN = isbn ?? throw new ArgumentNullException(nameof(isbn));
			if (totalCopies < 1) throw new ArgumentOutOfRangeException(nameof(totalCopies));
			TotalCopies = totalCopies;
			CopiesAvailable = totalCopies;
		}

		public Guid Id { get; private set; }

		public string Title { get; private set; } = string.Empty;
		public string? Subtitle { get; private set; }
		public string ISBN { get; private set; } = string.Empty;
		public string? Publisher { get; private set; }
		public int? PublishedYear { get; private set; }

		public int TotalCopies { get; private set; } = 1;
		public int CopiesAvailable { get; private set; } = 1;

		public Guid? CategoryId { get; private set; }
		public Category? Category { get; private set; }

		public Guid AuthorId { get; private set; }
		public Author Author { get; private set; } = null!;

		public Guid? LocationId { get; private set; }
		public Location? Location { get; private set; }

		public IReadOnlyCollection<BorrowRecord> BorrowRecords => _borrowRecords.AsReadOnly();

		public bool IsAvailable => CopiesAvailable > 0;

		public void AssignCategory(Category category)
		{
			Category = category ?? throw new ArgumentNullException(nameof(category));
			CategoryId = category.Id;
			category.AddBook(this);
		}

		public void AssignAuthor(Author author)
		{
			Author = author ?? throw new ArgumentNullException(nameof(author));
			AuthorId = author.Id;
			author.AddBook(this);
		}

		public void AssignLocation(Location location)
		{
			Location = location ?? throw new ArgumentNullException(nameof(location));
			LocationId = location.Id;
			location.AddBook(this);
		}

		public BorrowRecord Borrow(Guid userId, DateTime dueDate)
		{
			if (dueDate <= DateTime.UtcNow) throw new ArgumentException("Due date must be in the future.", nameof(dueDate));
			if (CopiesAvailable <= 0) throw new InvalidOperationException("No copies available.");

			CopiesAvailable--;
			var record = new BorrowRecord(Id, userId, DateTime.UtcNow, dueDate);
			_borrowRecords.Add(record);
			return record;
		}

		public void Return(BorrowRecord record)
		{
			if (record == null) throw new ArgumentNullException(nameof(record));
			if (!_borrowRecords.Contains(record)) throw new InvalidOperationException("Borrow record does not belong to this book.");
			if (record.ReturnDate.HasValue) return;

			record.MarkReturned();
			if (CopiesAvailable >= TotalCopies) throw new InvalidOperationException("All copies are already returned.");
			CopiesAvailable++;
		}
	}
}