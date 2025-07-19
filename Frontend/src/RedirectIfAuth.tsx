import { Navigate, Outlet } from 'react-router-dom';
import { session } from './session';

export default function RedirectIfAuth() {
  const user = session.getUser();
  return user ? <Navigate to="/" replace /> : <Outlet />;
}