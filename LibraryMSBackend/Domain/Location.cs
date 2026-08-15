using System;
using System.Collections.Generic;

namespace Domain
{
    public class Location
    {
        private readonly List<Book> _books = new();

        protected Location() { }

        public Location(string name, double latitude, double longitude)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Latitude = latitude;
            Longitude = longitude;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

        internal void AddBook(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (!_books.Contains(book)) _books.Add(book);
        }
    }
}