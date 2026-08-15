import { useState, useEffect, FormEvent } from 'react';
import api from '../api';
import { Author, Category, Location } from '../types';

interface AddBookFormProps {
  onAdded: () => void;
}

export default function AddBookForm({ onAdded }: AddBookFormProps) {
  const [title, setTitle] = useState('');
  const [isbn, setIsbn] = useState('');
  const [totalCopies, setTotalCopies] = useState('1');
  const [authorId, setAuthorId] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [locationId, setLocationId] = useState('');
  const [authors, setAuthors] = useState<Author[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    api.get('/Authors').then((res) => setAuthors(res.data));
    api.get('/Categories').then((res) => setCategories(res.data));
    api.get('/Locations').then((res) => setLocations(res.data));
  }, []);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');

    try {
      await api.post('/Books', {
        title,
        isbn,
        totalCopies: Number(totalCopies),
        authorId,
        categoryId,
        locationId: locationId || null,
      });
      setMessage('Book added successfully!');
      setTitle('');
      setIsbn('');
      setTotalCopies('1');
      setAuthorId('');
      setCategoryId('');
      setLocationId('');
      onAdded();
    } catch (err) {
      setError('Failed to add book. Check all fields are filled correctly.');
    }
  };

  return (
    <div>
      <h2>Add Book</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Title</label>
          <input value={title} onChange={(e) => setTitle(e.target.value)} required />
        </div>
        <div>
          <label>ISBN</label>
          <input value={isbn} onChange={(e) => setIsbn(e.target.value)} required />
        </div>
        <div>
          <label>Total Copies</label>
          <input
            type="number"
            min="1"
            value={totalCopies}
            onChange={(e) => setTotalCopies(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Author</label>
          <select value={authorId} onChange={(e) => setAuthorId(e.target.value)} required>
            <option value="">Select an author</option>
            {authors.map((a) => (
              <option key={a.id} value={a.id}>{a.fullName}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Category</label>
          <select value={categoryId} onChange={(e) => setCategoryId(e.target.value)} required>
            <option value="">Select a category</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
        </div>
        <div>
          <label>Location (optional)</label>
          <select value={locationId} onChange={(e) => setLocationId(e.target.value)}>
            <option value="">No location</option>
            {locations.map((l) => (
              <option key={l.id} value={l.id}>{l.name}</option>
            ))}
          </select>
        </div>
        {message && <p style={{ color: 'green' }}>{message}</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit">Add Book</button>
      </form>
    </div>
  );
}