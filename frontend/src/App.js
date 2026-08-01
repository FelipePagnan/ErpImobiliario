import React from 'react';
import { BrowserRouter, Routes, Route, Navigate, NavLink, useLocation } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import PropertyList from './pages/PropertyList';
import PropertyDetail from './pages/PropertyDetail';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import AdminContracts from './pages/admin/AdminContracts';
import AdminFinancial from './pages/admin/AdminFinancial';
import AdminVisits from './pages/admin/AdminVisits';
import AdminCrm from './pages/admin/AdminCrm';
import './styles/global.css';

function ProtectedRoute({ children, roles }) {
  const { usuario, loading } = useAuth();
  if (loading) return <div className="loading" style={{ padding: '4rem' }}>Carregando...</div>;
  if (!usuario) return <Navigate to="/login" />;
  if (roles && !roles.includes(usuario.perfil)) return <Navigate to="/" />;
  return children;
}

function AdminNav() {
  const { usuario } = useAuth();
  const location = useLocation();
  if (!usuario || !['Administrador', 'Gerente', 'Corretor'].includes(usuario.perfil)) return null;

  const isAdmin = location.pathname.startsWith('/admin');
  if (!isAdmin) return null;

  const links = [
    { to: '/admin/dashboard', label: 'Dashboard' },
    { to: '/admin/visitas', label: 'Visitas' },
    { to: '/admin/contratos', label: 'Contratos', roles: ['Administrador', 'Gerente'] },
    { to: '/admin/financeiro', label: 'Financeiro', roles: ['Administrador', 'Gerente'] },
    { to: '/admin/crm', label: 'CRM' },
  ];

  return (
    <nav className="admin-nav container">
      {links.filter(l => !l.roles || l.roles.includes(usuario.perfil)).map(l => (
        <NavLink key={l.to} to={l.to} className={({ isActive }) => isActive ? 'active' : ''}>
          {l.label}
        </NavLink>
      ))}
    </nav>
  );
}

function AppRoutes() {
  const adminRoles = ['Administrador', 'Gerente', 'Corretor'];
  const managerRoles = ['Administrador', 'Gerente'];

  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/imoveis" element={<PropertyList />} />
      <Route path="/imoveis/:id" element={<PropertyDetail />} />
      <Route path="/login" element={<Login />} />
      <Route path="/admin/dashboard" element={<ProtectedRoute roles={adminRoles}><Dashboard /></ProtectedRoute>} />
      <Route path="/admin/visitas" element={<ProtectedRoute roles={adminRoles}><AdminVisits /></ProtectedRoute>} />
      <Route path="/admin/contratos" element={<ProtectedRoute roles={managerRoles}><AdminContracts /></ProtectedRoute>} />
      <Route path="/admin/financeiro" element={<ProtectedRoute roles={managerRoles}><AdminFinancial /></ProtectedRoute>} />
      <Route path="/admin/crm" element={<ProtectedRoute roles={adminRoles}><AdminCrm /></ProtectedRoute>} />
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="app-wrapper">
          <Header />
          <AdminNav />
          <main className="app-main">
            <AppRoutes />
          </main>
          <Footer />
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}
