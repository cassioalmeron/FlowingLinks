import { BrowserRouter as Router, Routes, Route, Link, useNavigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Home from './Pages/Home/Index';
import Login from './Pages/Login/Index';
import Users from './Pages/Users/Index';
import './App.css';
import Projects from './Pages/Projects/Index';
import RequireAuth from './RequireAuth';
import RedirectIfAuth from './RedirectIfAuth';
import Tests from './Pages/Tests';
import { session } from './session';
import Profile from './Pages/Profile';
import Labels from './Pages/Labels';
import Links from './Pages/Links';

// Component to conditionally render layout
const AppLayout = () => {
  const navigate = useNavigate();
  const user = session.getUser();

  const handleLogout = () => {
    session.logout();
    navigate('/login');
  };

  return (
    <div className="app-layout">
      {/* Sidebar Navigation */}
      <nav className="sidebar">
        <div className="sidebar-links">
          <Link to="/" className="sidebar-link">🏠 Home</Link>
          {!user?.isAdmin && 
            <>
              <Link to="/links" className="sidebar-link">🔗 Links</Link>
              <Link to="/labels" className="sidebar-link">🏷️ Labels</Link>
            </>
          }
          {user?.isAdmin && <Link to="/users" className="sidebar-link">👥 Users</Link>}
          <Link to="/profile" className="sidebar-link">👤 Profile</Link>
        </div>
        <button className="sidebar-link logout-btn" onClick={handleLogout} style={{ marginTop: 'auto', width: '100%' }}>
          🚪 Logout
        </button>
      </nav>
      {/* Main Content Area */}
      <main className="main-content">
        <Routes>
          <Route element={<RequireAuth />}>
            <Route path="/" element={<Home />} />
            <Route path="/users" element={<Users />} />
            <Route path="/projects" element={<Projects />} />
            <Route path="/tests" element={<Tests />} />
            <Route path="/labels" element={<Labels />} />
            <Route path="/profile" element={<Profile />} />
            <Route path="/links" element={<Links />} />
          </Route>
        </Routes>
      </main>
    </div>
  );
};

function App() {
  return (
    <Router>
      <Routes>
        {/* Login route with RedirectIfAuth */}
        <Route element={<RedirectIfAuth />}>
          <Route path="/login" element={<Login />} />
        </Route>
        
        {/* Protected routes with RequireAuth */}
        <Route element={<RequireAuth />}>
          <Route path="/*" element={<AppLayout />} />
        </Route>
      </Routes>
      <ToastContainer />
    </Router>
  );
}

export default App;
