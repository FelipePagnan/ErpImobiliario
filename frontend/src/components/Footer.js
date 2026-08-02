import React from 'react';
import './Footer.css';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-inner">
        <div className="footer-brand">
          <span className="footer-logo">⌂ Pagnan<span className="logo-accent"> Hub</span> Imóveis</span>
          <p className="footer-desc">Plataforma completa de gestão e busca imobiliária.</p>
        </div>
        <div className="footer-copy">
          <p>&copy; {new Date().getFullYear()} Pagnan Hub Imóveis. Todos os direitos reservados.</p>
        </div>
      </div>
    </footer>
  );
}
