import { useState, useEffect } from 'react';
import api from './api';
import { BorrowRecord } from './types';

export default function BorrowHistory() {
  const [records, setRecords] = useState<BorrowRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [returningId, setReturningId] = useState<string | null>(null);

  useEffect(() => {
    loadHistory();
  }, []);

  const loadHistory = () => {
    setLoading(true);
    api.get('/Books/my-history')
      .then((response) => {
        setRecords(response.data);
        setLoading(false);
      })
      .catch(() => {
        setError('Failed to load borrow history.');
        setLoading(false);
      });
  };

  const handleReturn = async (recordId: string) => {
    setReturningId(recordId);
    setError('');
    try {
      await api.post(`/Books/return/${recordId}`);
      loadHistory();
    } catch (err) {
      setError('Failed to return book.');
    } finally {
      setReturningId(null);
    }
  };

  if (loading) return <p>Loading history...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div className="page-content">
      <h2>My Borrow History</h2>
      {records.length === 0 ? (
        <p>You haven't borrowed any books yet.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Book</th>
              <th>Borrow Date</th>
              <th>Due Date</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {records.map((r) => (
              <tr key={r.id}>
                <td>{r.bookTitle}</td>
                <td>{new Date(r.borrowDate).toLocaleDateString()}</td>
                <td>{new Date(r.dueDate).toLocaleDateString()}</td>
                <td>{r.status}</td>
                <td>
                  {r.status === 'Borrowed' || r.status === 'Overdue' ? (
                    <button
                      onClick={() => handleReturn(r.id)}
                      disabled={returningId === r.id}
                    >
                      {returningId === r.id ? 'Returning...' : 'Return'}
                    </button>
                  ) : (
                    '—'
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}