import { useState } from 'react';
import api from '../api';

interface ManageListProps<T> {
  title: string;
  endpoint: string;
  items: T[];
  onDeleted: (id: string) => void;
  getId: (item: T) => string;
  renderLabel: (item: T) => string;
}

export default function ManageList<T>({
  title, endpoint, items, onDeleted, getId, renderLabel,
}: ManageListProps<T>) {
  const [error, setError] = useState('');

  const handleDelete = async (id: string) => {
    if (!confirm('Delete this item? This cannot be undone.')) return;
    setError('');
    try {
      await api.delete(`${endpoint}/${id}`);
      onDeleted(id);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete.');
    }
  };

  return (
    <div style={{ marginTop: '1rem' }}>
      <h4>{title}</h4>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {items.length === 0 ? (
        <p style={{ color: 'var(--text2)' }}>None yet.</p>
      ) : (
        <table>
          <tbody>
            {items.map((item) => (
              <tr key={getId(item)}>
                <td>{renderLabel(item)}</td>
                <td style={{ textAlign: 'right' }}>
                  <button onClick={() => handleDelete(getId(item))} style={{ background: 'var(--pink)' }}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}