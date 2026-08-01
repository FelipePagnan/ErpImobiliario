import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import PropertyList from './pages/PropertyList';
import PropertyDetail from './pages/PropertyDetail';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import './styles/global.css';

function ProtectedRoute({ children, roles }) {
  const { usuario, loading } = useAuth();

  if (loading) return <div className="loading" style={{ padding: '4rem' }}>Carregando...</div>;
  if (!usuario) return <Navigate to="/login" />;
  if (roles && !roles.includes(usuario.perfil)) return <Navigate to="/" />;

  return children;
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/imoveis" element={<PropertyList />} />
      <Route path="/imoveis/:id" element={<PropertyDetail />} />
      <Route path="/login" element={<Login />} />
      <Route
        path="/admin/dashboard"
        element={
          <ProtectedRoute roles={['Administrador', 'Gerente', 'Corretor']}>
            <Dashboard />
          </ProtectedRoute>
        }
      />
    </Routes>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <div className="app-wrapper">
          <Header />
          <main className="app-main">
            <AppRoutes />
          </main>
          <Footer />
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}
