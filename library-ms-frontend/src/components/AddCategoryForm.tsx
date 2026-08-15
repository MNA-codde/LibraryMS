import { useState, FormEvent } from 'react';
import api from '../api';

interface AddCategoryFormProps {
  onAdded: () => void;
}

export default function AddCategoryForm({ onAdded }: AddCategoryFormProps) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');

    try {
      await api.post('/Categories', { name, description });
      setMessage('Category added successfully!');
      setName('');
      setDescription('');
      onAdded();
    } catch (err) {
      setError('Failed to add category.');
    }
  };

  return (
    <div>
      <h3>Add Category</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Name</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div>
          <label>Description (optional)</label>
          <input value={description} onChange={(e) => setDescription(e.target.value)} />
        </div>
        {message && <p style={{ color: 'green' }}>{message}</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit">Add Category</button>
      </form>
    </div>
  );
}