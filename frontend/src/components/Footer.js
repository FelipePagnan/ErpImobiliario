import React from 'react';
import './Footer.css';

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer-inner">
        <div className="footer-brand">
          <span className="footer-logo">⌂ Imobiliária<span className="logo-accent">ERP</span></span>
          <p className="footer-desc">Sistema de gestão imobiliária completo para sua empresa.</p>
        </div>
        <div className="footer-copy">
          <p>&copy; {new Date().getFullYear()} ImobiliáriaERP. Projeto educacional.</p>
        </div>
      </div>
    </footer>
  );
}
