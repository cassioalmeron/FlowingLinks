import { Navigate, Outlet } from 'react-router-dom';
import { session } from './session';

export default function RequireAuth() {
  const isAuthenticated = session.isAuthenticated();
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
} 