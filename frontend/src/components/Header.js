import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Header.css';

export default function Header() {
  const { usuario, logout } = useAuth();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/');
    setMenuOpen(false);
  };

  return (
    <header className="header">
      <div className="header-inner container">
        <Link to="/" className="header-logo">
          <span className="logo-icon">⌂</span>
          <span className="logo-text">Imobiliária<span className="logo-accent">ERP</span></span>
        </Link>

        <button className="menu-toggle" onClick={() => setMenuOpen(!menuOpen)} aria-label="Menu">
          <span className={`hamburger ${menuOpen ? 'open' : ''}`} />
        </button>

        <nav className={`header-nav ${menuOpen ? 'open' : ''}`}>
          <Link to="/" onClick={() => setMenuOpen(false)}>Início</Link>
          <Link to="/imoveis" onClick={() => setMenuOpen(false)}>Imóveis</Link>

          {usuario ? (
            <>
              {(usuario.perfil === 'Administrador' || usuario.perfil === 'Gerente' || usuario.perfil === 'Corretor') && (
                <Link to="/admin/dashboard" onClick={() => setMenuOpen(false)}>Painel</Link>
              )}
              <div className="header-user">
                <span className="user-name">{usuario.nome}</span>
                <button className="btn btn-outline btn-sm" onClick={handleLogout}>Sair</button>
              </div>
            </>
          ) : (
            <Link to="/login" className="btn btn-primary btn-sm" onClick={() => setMenuOpen(false)}>Entrar</Link>
          )}
        </nav>
      </div>
    </header>
  );
}
