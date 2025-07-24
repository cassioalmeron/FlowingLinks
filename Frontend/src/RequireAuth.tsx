import { Navigate, Outlet } from 'react-router-dom';
import { session } from './session';

export default function RequireAuth() {
  const user = session.getUser();
  return user ? <Outlet /> : <Navigate to="/login" replace />;
} 