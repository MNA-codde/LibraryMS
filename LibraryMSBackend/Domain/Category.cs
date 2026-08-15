using System;
using System.Collections.Generic;

namespace Domain
{
	public class Category
	{
		private readonly List<Book> _books = new();

		protected Category() { }

		public Category(string name)
		{
			Id = Guid.NewGuid();
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public Guid Id { get; private set; }

		public string Name { get; private set; } = string.Empty;
		public string? Description { get; private set; }

		public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

		public void SetDescription(string? description) => Description = description;

		internal void AddBook(Book book)
		{
			if (book == null) throw new ArgumentNullException(nameof(book));
			if (!_books.Contains(book)) _books.Add(book);
		}
	}
}

