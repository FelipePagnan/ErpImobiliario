import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useToast } from '../components/Toast';
import './Login.css';

export default function Login() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const toast = useToast();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const data = await login(email, senha);
      toast.success(`Bem-vindo, ${data.usuario.nome}!`);
      const perfil = data.usuario.perfil;
      if (['Administrador','Gerente','Corretor'].includes(perfil)) navigate('/admin/dashboard');
      else navigate('/');
    } catch (err) {
      toast.error(err.response?.data?.mensagem || 'E-mail ou senha inválidos.');
    } finally { setLoading(false); }
  };

  return (
    <div className="login-page">
      <div className="login-card fade-in">
        <div className="login-header">
          <span className="login-logo">⌂</span>
          <h1>Entrar</h1>
          <p>Acesse sua conta na Pagnan Hub Imóveis</p>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-group"><label className="form-label" htmlFor="email">E-mail</label>
            <input id="email" type="email" className="form-input" placeholder="seu@email.com" value={email} onChange={e=>setEmail(e.target.value)} required /></div>
          <div className="form-group"><label className="form-label" htmlFor="senha">Senha</label>
            <input id="senha" type="password" className="form-input" placeholder="Sua senha" value={senha} onChange={e=>setSenha(e.target.value)} required /></div>
          <button type="submit" className="btn btn-primary login-btn" disabled={loading}>{loading ? 'Entrando...' : 'Entrar'}</button>
        </form>
        <div className="login-demo">
          <p>Contas de demonstração:</p>
          <div className="demo-accounts">
            <button onClick={()=>{setEmail('admin@imobiliaria.com');setSenha('admin123');}}>Admin</button>
            <button onClick={()=>{setEmail('carlos@imobiliaria.com');setSenha('corretor123');}}>Corretor</button>
            <button onClick={()=>{setEmail('joao@email.com');setSenha('cliente123');}}>Cliente</button>
          </div>
        </div>
      </div>
    </div>
  );
}
