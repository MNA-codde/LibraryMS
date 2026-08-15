import { useState, FormEvent } from 'react';
import api from '../api';

interface AddAuthorFormProps {
  onAdded: () => void;
}

export default function AddAuthorForm({ onAdded }: AddAuthorFormProps) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');

    try {
      await api.post('/Authors', { firstName, lastName });
      setMessage('Author added successfully!');
      setFirstName('');
      setLastName('');
      onAdded();
    } catch (err) {
      setError('Failed to add author.');
    }
  };

  return (
    <div>
      <h3>Add Author</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label>First Name</label>
          <input value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
        </div>
        <div>
          <label>Last Name</label>
          <input value={lastName} onChange={(e) => setLastName(e.target.value)} required />
        </div>
        {message && <p style={{ color: 'green' }}>{message}</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit">Add Author</button>
      </form>
    </div>
  );
}