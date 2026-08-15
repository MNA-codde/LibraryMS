import { NavLink, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import AdminNotifications from './AdminNotifications';

export default function NavBar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav>
      <Link to={user ? '/dashboard' : '/'} style={{ display: 'flex', alignItems: 'center' }}>
        <svg width="28" height="28" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" stroke="var(--text)" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
          <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z" stroke="var(--text)" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
        </svg>
      </Link>

      <label className="theme-switch">
        <input type="checkbox" id="theme-toggle" />
        <span className="theme-switch-btn">
          <span className="icon-sun">☀️</span>
          <span className="icon-moon">🌙</span>
        </span>
      </label>

      {user && <NavLink to="/dashboard">Dashboard</NavLink>}
      <NavLink to="/books">Books</NavLink>
      {user?.role === 'Member' && <NavLink to="/my-history">My Borrow History</NavLink>}
      {user?.role === 'Admin' && <NavLink to="/admin/add-book">Manage Catalog</NavLink>}

      <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '1rem' }}>
  {user ? (
    <>
      <span>Logged in as {user.fullName} ({user.role})</span>
      <AdminNotifications />
      <button onClick={handleLogout}>Log out</button>
    </>
  ) : (
    <>
      <Link to="/login">Log in</Link> | <Link to="/register">Register</Link>
    </>
  )}
</div>
    </nav>
  );
}