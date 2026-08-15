import { Link, Navigate } from 'react-router-dom';
import { useAuth } from '../AuthContext';

export default function LandingPage() {
  const { user } = useAuth();

  if (user) {
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <div style={{ display: 'flex', minHeight: 'calc(100vh - 64px)' }}>
      <div
        style={{
          flex: 1,
          backgroundImage:
            'url(https://images.unsplash.com/photo-1521587760476-6c12a4b040da?q=80&w=1200)',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      />
      <div style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <div style={{ maxWidth: 420, textAlign: 'center' }}>
          <h1 style={{ marginBottom: '0.5rem' }}>LibraryMS</h1>
          <p style={{ color: 'var(--text2)', marginBottom: '2rem' }}>
            Browse the catalog, borrow books, and track your reading — all in one place.
          </p>
          <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
            <Link to="/login">
              <button type="submit">Log In</button>
            </Link>
            <Link to="/register">
              <button type="submit" style={{ backgroundColor: 'var(--purple)' }}>Register</button>
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}