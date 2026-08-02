import React from 'react';
import { BrowserRouter, Routes, Route, Navigate, NavLink, useLocation } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ToastProvider } from './components/Toast';
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
import AdminProperties from './pages/admin/AdminProperties';
import AdminAudit from './pages/admin/AdminAudit';
import ClientVisits from './pages/client/ClientVisits';
import ClientFavorites from './pages/client/ClientFavorites';
import './styles/global.css';

function ProtectedRoute({ children, roles }) {
  const { usuario, loading } = useAuth();
  if (loading) return <div className="loading" style={{padding:'4rem'}}>Carregando...</div>;
  if (!usuario) return <Navigate to="/login" />;
  if (roles && !roles.includes(usuario.perfil)) return <Navigate to="/" />;
  return children;
}

function AdminNav() {
  const { usuario } = useAuth();
  const location = useLocation();
  if (!usuario || !['Administrador','Gerente','Corretor'].includes(usuario.perfil) || !location.pathname.startsWith('/admin')) return null;
  const links = [
    {to:'/admin/dashboard',label:'Dashboard'},{to:'/admin/imoveis',label:'Imóveis'},
    {to:'/admin/visitas',label:'Visitas'},{to:'/admin/contratos',label:'Contratos',roles:['Administrador','Gerente']},
    {to:'/admin/financeiro',label:'Financeiro',roles:['Administrador','Gerente']},{to:'/admin/crm',label:'CRM'},
    {to:'/admin/auditoria',label:'Auditoria',roles:['Administrador']},
  ];
  return <nav className="admin-nav container">{links.filter(l=>!l.roles||l.roles.includes(usuario.perfil)).map(l=><NavLink key={l.to} to={l.to} className={({isActive})=>isActive?'active':''}>{l.label}</NavLink>)}</nav>;
}

function ClientNav() {
  const { usuario } = useAuth();
  const location = useLocation();
  if (!usuario || !location.pathname.startsWith('/minha-conta')) return null;
  return <nav className="admin-nav container"><NavLink to="/minha-conta/visitas" className={({isActive})=>isActive?'active':''}>Minhas Visitas</NavLink><NavLink to="/minha-conta/favoritos" className={({isActive})=>isActive?'active':''}>Favoritos</NavLink></nav>;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/imoveis" element={<PropertyList />} />
      <Route path="/imoveis/:id" element={<PropertyDetail />} />
      <Route path="/login" element={<Login />} />
      <Route path="/admin/dashboard" element={<ProtectedRoute roles={['Administrador','Gerente','Corretor']}><Dashboard /></ProtectedRoute>} />
      <Route path="/admin/imoveis" element={<ProtectedRoute roles={['Administrador','Gerente']}><AdminProperties /></ProtectedRoute>} />
      <Route path="/admin/visitas" element={<ProtectedRoute roles={['Administrador','Gerente','Corretor']}><AdminVisits /></ProtectedRoute>} />
      <Route path="/admin/contratos" element={<ProtectedRoute roles={['Administrador','Gerente']}><AdminContracts /></ProtectedRoute>} />
      <Route path="/admin/financeiro" element={<ProtectedRoute roles={['Administrador','Gerente']}><AdminFinancial /></ProtectedRoute>} />
      <Route path="/admin/crm" element={<ProtectedRoute roles={['Administrador','Gerente','Corretor']}><AdminCrm /></ProtectedRoute>} />
      <Route path="/admin/auditoria" element={<ProtectedRoute roles={['Administrador']}><AdminAudit /></ProtectedRoute>} />
      <Route path="/minha-conta/visitas" element={<ProtectedRoute roles={['Cliente']}><ClientVisits /></ProtectedRoute>} />
      <Route path="/minha-conta/favoritos" element={<ProtectedRoute roles={['Cliente']}><ClientFavorites /></ProtectedRoute>} />
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastProvider>
          <div className="app-wrapper">
            <Header />
            <AdminNav />
            <ClientNav />
            <main className="app-main"><AppRoutes /></main>
            <Footer />
          </div>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
