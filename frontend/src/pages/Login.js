import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Login.css';

export default function Login() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErro('');
    setLoading(true);

    try {
      const data = await login(email, senha);
      const perfil = data.usuario.perfil;
      if (perfil === 'Administrador' || perfil === 'Gerente' || perfil === 'Corretor') {
        navigate('/admin/dashboard');
      } else {
        navigate('/');
      }
    } catch (err) {
      setErro(err.response?.data?.mensagem || 'Erro ao fazer login. Verifique suas credenciais.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card fade-in">
        <div className="login-header">
          <span className="login-logo">⌂</span>
          <h1>Entrar</h1>
          <p>Acesse sua conta no ImobiliáriaERP</p>
        </div>

        <form onSubmit={handleSubmit}>
          {erro && <div className="login-error">{erro}</div>}

          <div className="form-group">
            <label className="form-label" htmlFor="email">E-mail</label>
            <input
              id="email"
              type="email"
              className="form-input"
              placeholder="seu@email.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="senha">Senha</label>
            <input
              id="senha"
              type="password"
              className="form-input"
              placeholder="Sua senha"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              required
            />
          </div>

          <button type="submit" className="btn btn-primary login-btn" disabled={loading}>
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="login-demo">
          <p>Contas de demonstração:</p>
          <div className="demo-accounts">
            <button onClick={() => { setEmail('admin@imobiliaria.com'); setSenha('admin123'); }}>
              Admin
            </button>
            <button onClick={() => { setEmail('carlos@imobiliaria.com'); setSenha('corretor123'); }}>
              Corretor
            </button>
            <button onClick={() => { setEmail('joao@email.com'); setSenha('cliente123'); }}>
              Cliente
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
