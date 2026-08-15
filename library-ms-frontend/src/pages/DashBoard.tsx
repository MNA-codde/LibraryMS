import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import api from '../api';
import { useAuth } from '../AuthContext';
import { Book, BorrowRecord, BookPopularity, LocationStats } from '../types';
import LocationMap from '../components/LocationMap';
import {
  BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid,
  PieChart, Pie, Cell, Legend,
} from 'recharts';

const COLORS = ['#9333EA', '#0EA5E9', '#F91880', '#06B6D4', '#10B981', '#F97316'];

export default function Dashboard() {
  const { user } = useAuth();
  const [books, setBooks] = useState<Book[]>([]);
  const [history, setHistory] = useState<BorrowRecord[]>([]);
  const [popularity, setPopularity] = useState<BookPopularity[]>([]);
  const [locationStats, setLocationStats] = useState<LocationStats[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const requests: Promise<any>[] = [api.get('/Books')];
    if (user?.role === 'Member') {
      requests.push(api.get('/Books/my-history'));
    }
    if (user?.role === 'Admin') {
      requests.push(api.get('/Stats/popularity'));
      requests.push(api.get('/Locations/stats'));
    }

    Promise.all(requests)
      .then((results) => {
        setBooks(results[0].data);
        if (user?.role === 'Member') setHistory(results[1].data);
        if (user?.role === 'Admin') {
          setPopularity(results[1].data);
          setLocationStats(results[2].data);
        }
        setLoading(false);
      })
      .catch(() => setLoading(false));
  }, [user]);

  if (loading) return <p>Loading dashboard...</p>;

  const totalBooks = books.length;
  const totalCopies = books.reduce((sum, b) => sum + b.totalCopies, 0);
  const availableCopies = books.reduce((sum, b) => sum + b.copiesAvailable, 0);
  const borrowedCopies = totalCopies - availableCopies;

  const activeBorrows = history.filter((r) => r.status === 'Borrowed').length;
  const overdueBorrows = history.filter((r) => r.status === 'Overdue').length;

  const buttonStyle = {
    padding: '0.7rem 1.4rem',
    borderRadius: '10px',
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    color: 'var(--text)',
    textDecoration: 'none',
    fontWeight: 600,
  };

  const chartCardStyle = {
    background: 'var(--bg2)',
    border: '1px solid var(--border)',
    borderRadius: '14px',
    padding: '1.5rem',
  };

  return (
    <div className="page-content">
      <h2>Dashboard</h2>
      <div style={{ display: 'flex', gap: '2.5rem', flexWrap: 'wrap', margin: '1.5rem 0' }}>
        <StatCard label="Total Books" value={totalBooks} icon="📚" gradient="linear-gradient(135deg, var(--blue), var(--purple))" detail={`${totalCopies} total copies across your catalog.`} />
        <StatCard label="Copies Available" value={availableCopies} icon="✅" gradient="linear-gradient(135deg, var(--green), var(--teal))" detail={`${availableCopies} of ${totalCopies} copies are ready to borrow.`} />
        <StatCard label="Copies Borrowed" value={borrowedCopies} icon="📖" gradient="linear-gradient(135deg, var(--orange), var(--pink))" detail={`${borrowedCopies} copies are currently checked out.`} />
        {user?.role === 'Member' && (
          <>
            <StatCard label="Your Active Borrows" value={activeBorrows} icon="🔖" gradient="linear-gradient(135deg, var(--purple), var(--pink))" detail="Books currently checked out under your account." />
            <StatCard label="Overdue" value={overdueBorrows} icon="⏰" gradient="linear-gradient(135deg, var(--pink), var(--orange))" detail={overdueBorrows > 0 ? "Return these as soon as possible." : "You're all caught up — nothing overdue."} />
          </>
        )}
      </div>

      {user?.role === 'Admin' && popularity.length > 0 && (
        <div style={{ marginBottom: '2rem' }}>
          <h3>Book Popularity</h3>
          <div style={chartCardStyle}>
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={popularity.slice(0, 8)}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="title" tick={{ fill: 'var(--text2)', fontSize: 11 }} angle={-20} textAnchor="end" height={60} />
                <YAxis tick={{ fill: 'var(--text2)' }} allowDecimals={false} />
                <Tooltip contentStyle={{ background: 'var(--bg)', border: '1px solid var(--border)', borderRadius: 8, color: 'var(--text)' }} />
                <Bar dataKey="borrowCount" fill="var(--purple)" radius={[6, 6, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      )}

      {user?.role === 'Admin' && locationStats.length > 0 && (
        <div style={{ marginBottom: '2rem', display: 'flex', gap: '1.5rem', flexWrap: 'wrap' }}>
          <div style={{ flex: '1 1 400px' }}>
            <h3>Locations</h3>
            <LocationMap locations={locationStats} />
          </div>
          <div style={{ flex: '1 1 300px' }}>
            <h3>Books by Location</h3>
            <div style={chartCardStyle}>
              <ResponsiveContainer width="100%" height={280}>
                <PieChart>
                  <Pie data={locationStats} dataKey="bookCount" nameKey="name" outerRadius={90} label>
                    {locationStats.map((_, i) => (
                      <Cell key={i} fill={COLORS[i % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip contentStyle={{ background: 'var(--bg)', border: '1px solid var(--border)', borderRadius: 8, color: 'var(--text)' }} />
                  <Legend wrapperStyle={{ color: 'var(--text2)' }} />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      )}

      <div style={{ display: 'flex', gap: '1rem', marginTop: '0.5rem' }}>
        <Link to="/books" style={buttonStyle}>Browse Books →</Link>
        {user?.role === 'Member' && <Link to="/my-history" style={buttonStyle}>View Borrow History →</Link>}
        {user?.role === 'Admin' && <Link to="/admin/add-book" style={buttonStyle}>Manage Catalog →</Link>}
      </div>
    </div>
  );
}

interface StatCardProps {
  label: string;
  value: number;
  icon: string;
  gradient: string;
  detail: string;
}

function StatCard({ label, value, icon, gradient, detail }: StatCardProps) {
  const [hovered, setHovered] = useState(false);

  return (
    <div
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      style={{
        position: 'relative', background: 'var(--bg2)', border: '1px solid var(--border)',
        borderRadius: '14px', padding: '1.5rem', minWidth: '170px', textAlign: 'center', cursor: 'default',
        transition: 'transform 0.15s ease, box-shadow 0.15s ease',
        transform: hovered ? 'translateY(-3px)' : 'none',
        boxShadow: hovered ? '0 8px 20px rgba(0,0,0,0.15)' : 'none',
      }}
    >
      <div style={{ width: '56px', height: '56px', borderRadius: '50%', background: gradient, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.5rem', margin: '0 auto 0.8rem' }}>
        {icon}
      </div>
      <div style={{ fontSize: '1.6rem', fontWeight: 700, color: 'var(--text)' }}>{value}</div>
      <div style={{ fontSize: '0.85rem', color: 'var(--text2)' }}>{label}</div>

      {hovered && (
        <div style={{ position: 'absolute', top: 'calc(100% + 8px)', left: '50%', transform: 'translateX(-50%)', width: '220px', background: 'var(--bg)', border: '1px solid var(--border)', borderRadius: '10px', padding: '0.8rem 1rem', fontSize: '0.8rem', color: 'var(--text2)', boxShadow: '0 4px 12px rgba(0,0,0,0.15)', zIndex: 10 }}>
          {detail}
        </div>
      )}
    </div>
  );
}