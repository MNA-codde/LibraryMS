using System;
using System.Collections.Generic;

namespace Domain
{
	public class Author
	{
		private readonly List<Book> _books = new();

		protected Author() { }

		public Author(string firstName, string lastName)
		{
			Id = Guid.NewGuid();
			FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
			LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
		}

		public Guid Id { get; private set; }

		public string FirstName { get; private set; } = string.Empty;
		public string LastName { get; private set; } = string.Empty;

		public string? Bio { get; private set; }
		public DateTime? DateOfBirth { get; private set; }

		public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

		public string FullName => string.Join(' ', new[] { FirstName, LastName }).Trim();

		public void SetBio(string? bio) => Bio = bio;

		public void SetDateOfBirth(DateTime? dob) => DateOfBirth = dob;

		internal void AddBook(Book book)
		{
			if (book == null) throw new ArgumentNullException(nameof(book));
			if (!_books.Contains(book)) _books.Add(book);
		}
	}
}

