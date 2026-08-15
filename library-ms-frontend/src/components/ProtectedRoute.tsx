import { Navigate } from 'react-router-dom';
import { ReactNode } from 'react';
import { useAuth } from '../AuthContext';

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRole?: 'Admin' | 'Member';
}

export default function ProtectedRoute({ children, requiredRole }: ProtectedRouteProps) {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole && user.role !== requiredRole) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}