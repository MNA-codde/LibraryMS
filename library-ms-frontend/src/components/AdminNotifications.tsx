import { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { useAuth } from '../AuthContext';

interface Notification {
  id: number;
  message: string;
  timestamp: Date;
}

export default function AdminNotifications() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [toasts, setToasts] = useState<Notification[]>([]);
  const [open, setOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (user?.role !== 'Admin') return;

    const token = localStorage.getItem('token');
    const connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5228/hubs/notifications', {
        accessTokenFactory: () => token || '',
      })
      .withAutomaticReconnect()
      .build();

    const addNotification = (message: string) => {
      const entry: Notification = { id: Date.now(), message, timestamp: new Date() };
      setNotifications((prev) => [entry, ...prev]);
      setToasts((prev) => [...prev, entry]);
      setTimeout(() => {
        setToasts((prev) => prev.filter((t) => t.id !== entry.id));
      }, 6000);
    };

    connection.on('BookBorrowed', (message: string) => addNotification(message));
    connection.on('BookReturned', (message: string) => addNotification(message));
    connection.on('OverdueSummary', (message: string) => addNotification(message));

    connection.start().catch((err) => console.error('SignalR connection failed:', err));

    return () => {
      connection.stop();
    };
  }, [user]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  if (user?.role !== 'Admin') return null;

  return (
    <>
      {/* Toasts */}
      <div
        style={{
          position: 'fixed',
          top: '80px',
          right: '20px',
          display: 'flex',
          flexDirection: 'column',
          gap: '0.5rem',
          zIndex: 1000,
        }}
      >
        {toasts.map((t) => (
          <div
            key={t.id}
            style={{
              background: 'var(--bg2)',
              border: '1px solid var(--border)',
              borderLeft: '4px solid var(--purple)',
              borderRadius: '8px',
              padding: '0.8rem 1.2rem',
              boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
              color: 'var(--text)',
              maxWidth: '320px',
            }}
          >
            🔔 {t.message}
          </div>
        ))}
      </div>

      {/* Bell icon + dropdown */}
      <div ref={dropdownRef} style={{ position: 'relative' }}>
        <button
          onClick={() => setOpen((o) => !o)}
          style={{
            background: 'var(--bg2)',
            border: '1px solid var(--border)',
            borderRadius: '10px',
            width: '40px',
            height: '40px',
            fontSize: '1.2rem',
            cursor: 'pointer',
                justifyContent: 'center',
    padding: 0,

            position: 'relative',
                flexShrink: 0,
    marginLeft: '-4px',
          }}
        >
          🔔
          {notifications.length > 0 && (
            <span
              style={{
                position: 'absolute',
                top: '-4px',
                right: '-4px',
                background: 'var(--pink)',
                color: 'white',
                borderRadius: '999px',
                fontSize: '0.7rem',
                fontWeight: 700,
                padding: '1px 6px',
                minWidth: '18px',
              }}
            >
              {notifications.length}
            </span>
          )}
        </button>

        {open && (
          <div
            style={{
              position: 'absolute',
              top: 'calc(100% + 8px)',
              right: 0,
              width: '320px',
              maxHeight: '400px',
              overflowY: 'auto',
              background: 'var(--bg2)',
              border: '1px solid var(--border)',
              borderRadius: '10px',
              boxShadow: '0 4px 16px rgba(0,0,0,0.2)',
              zIndex: 1000,
            }}
          >
            <div
              style={{
                padding: '0.8rem 1rem',
                borderBottom: '1px solid var(--border)',
                fontWeight: 600,
                color: 'var(--text)',
              }}
            >
              Notifications
            </div>
            {notifications.length === 0 ? (
              <div style={{ padding: '1rem', color: 'var(--text2)', fontSize: '0.9rem' }}>
                No notifications yet.
              </div>
            ) : (
              notifications.map((n) => (
                <div
                  key={n.id}
                  style={{
                    padding: '0.8rem 1rem',
                    borderBottom: '1px solid var(--border)',
                    fontSize: '0.85rem',
                    color: 'var(--text)',
                  }}
                >
                  {n.message}
                  <div style={{ fontSize: '0.75rem', color: 'var(--text2)', marginTop: '4px' }}>
                    {n.timestamp.toLocaleTimeString()}
                  </div>
                </div>
              ))
            )}
          </div>
        )}
      </div>
    </>
  );
}