import { useState, useEffect } from 'react';
import api from './api';
import { useAuth } from './AuthContext';
import { Book } from './types';

export default function BookList() {
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const { user } = useAuth();

  useEffect(() => {
    api.get('/Books')
      .then((response) => {
        setBooks(response.data);
        setLoading(false);
      })
      .catch(() => {
        setError('Failed to load books.');
        setLoading(false);
      });
  }, []);

  const handleBorrow = async (bookId: string) => {
    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 14);

    try {
      await api.post(`/Books/${bookId}/borrow`, JSON.stringify(dueDate.toISOString()), {
        headers: { 'Content-Type': 'application/json' },
      });
      alert('Book borrowed successfully!');
      const response = await api.get('/Books');
      setBooks(response.data);
    } catch (err) {
      alert('Failed to borrow book. Cannot Exceed 3 Books or Book is not available.');
    }
  };

  if (loading) return <p>Loading books...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div>
      <h2>Books</h2>
      <table>
        <thead>
          <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Category</th>
            <th>Available</th>
            {user?.role === 'Member' && <th></th>}
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.title}</td>
              <td>{book.authorName}</td>
              <td>{book.categoryName}</td>
              <td>{book.copiesAvailable} / {book.totalCopies}</td>
              {user?.role === 'Member' && (
                <td>
                  <button
                    onClick={() => handleBorrow(book.id)}
                    disabled={book.copiesAvailable === 0}
                  >
                    Borrow
                  </button>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}