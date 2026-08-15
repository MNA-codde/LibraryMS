import { useState, FormEvent } from 'react';
import api from '../api';

interface AddLocationFormProps {
  onAdded: () => void;
}

export default function AddLocationForm({ onAdded }: AddLocationFormProps) {
  const [name, setName] = useState('');
  const [latitude, setLatitude] = useState('');
  const [longitude, setLongitude] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');
    setMessage('');

    try {
      await api.post('/Locations', {
        name,
        latitude: Number(latitude),
        longitude: Number(longitude),
      });
      setMessage('Location added successfully!');
      setName('');
      setLatitude('');
      setLongitude('');
      onAdded();
    } catch (err) {
      setError('Failed to add location.');
    }
  };

  return (
    <div>
      <h3>Add Location</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Name</label>
          <input value={name} onChange={(e) => setName(e.target.value)} required />
        </div>
        <div>
          <label>Latitude</label>
          <input
            type="number"
            step="any"
            value={latitude}
            onChange={(e) => setLatitude(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Longitude</label>
          <input
            type="number"
            step="any"
            value={longitude}
            onChange={(e) => setLongitude(e.target.value)}
            required
          />
        </div>
        {message && <p style={{ color: 'green' }}>{message}</p>}
        {error && <p style={{ color: 'red' }}>{error}</p>}
        <button type="submit">Add Location</button>
      </form>
    </div>
  );
}   