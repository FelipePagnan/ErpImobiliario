import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { imoveisApi } from '../services/api';
import './Dashboard.css';

export default function Dashboard() {
  const { usuario } = useAuth();
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    imoveisApi.dashboard()
      .then(({ data }) => setDashboard(data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="dashboard-page">
        <div className="container">
          <div className="loading">Carregando painel...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <div className="container">
        <div className="dashboard-header">
          <div>
            <h1 className="dashboard-title">Painel Administrativo</h1>
            <p className="dashboard-welcome">
              Bem-vindo, <strong>{usuario?.nome}</strong>
              <span className="dashboard-role">{usuario?.perfil}</span>
            </p>
          </div>
          <Link to="/imoveis" className="btn btn-outline">Ver portal</Link>
        </div>

        {dashboard && (
          <>
            <div className="dashboard-cards">
              <div className="dash-card dash-card-primary">
                <span className="dash-card-icon">🏠</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.imoveisDisponiveis}</span>
                  <span className="dash-card-label">Disponíveis</span>
                </div>
              </div>
              <div className="dash-card dash-card-warning">
                <span className="dash-card-icon">🔑</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.imoveisAlugados}</span>
                  <span className="dash-card-label">Alugados</span>
                </div>
              </div>
              <div className="dash-card dash-card-success">
                <span className="dash-card-icon">✅</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.imoveisVendidos}</span>
                  <span className="dash-card-label">Vendidos</span>
                </div>
              </div>
              <div className="dash-card dash-card-info">
                <span className="dash-card-icon">👥</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.totalClientes}</span>
                  <span className="dash-card-label">Clientes</span>
                </div>
              </div>
              <div className="dash-card dash-card-accent">
                <span className="dash-card-icon">🤝</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.totalCorretores}</span>
                  <span className="dash-card-label">Corretores</span>
                </div>
              </div>
              <div className="dash-card">
                <span className="dash-card-icon">📋</span>
                <div className="dash-card-info">
                  <span className="dash-card-number">{dashboard.totalProprietarios}</span>
                  <span className="dash-card-label">Proprietários</span>
                </div>
              </div>
            </div>

            <section className="dashboard-section">
              <h2>Imóveis recentes</h2>
              {dashboard.imoveisRecentes && dashboard.imoveisRecentes.length > 0 ? (
                <div className="dashboard-table-wrapper">
                  <table className="dashboard-table">
                    <thead>
                      <tr>
                        <th>Código</th>
                        <th>Título</th>
                        <th>Tipo</th>
                        <th>Finalidade</th>
                        <th>Status</th>
                        <th>Preço</th>
                        <th>Corretor</th>
                      </tr>
                    </thead>
                    <tbody>
                      {dashboard.imoveisRecentes.map((imovel) => (
                        <tr key={imovel.id}>
                          <td><code>{imovel.codigo}</code></td>
                          <td>
                            <Link to={`/imoveis/${imovel.id}`} className="table-link">
                              {imovel.titulo}
                            </Link>
                          </td>
                          <td>{imovel.tipo}</td>
                          <td>
                            <span className={`badge badge-${imovel.finalidade === 'Venda' ? 'venda' : 'locacao'}`}>
                              {imovel.finalidade === 'Locacao' ? 'Locação' : imovel.finalidade}
                            </span>
                          </td>
                          <td>
                            <span className={`badge badge-${imovel.status.toLowerCase()}`}>
                              {imovel.status}
                            </span>
                          </td>
                          <td>
                            {imovel.precoVenda
                              ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(imovel.precoVenda)
                              : imovel.precoLocacao
                                ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(imovel.precoLocacao) + '/mês'
                                : '—'}
                          </td>
                          <td>{imovel.corretorNome || '—'}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <p className="empty-state">Nenhum imóvel cadastrado.</p>
              )}
            </section>
          </>
        )}
      </div>
    </div>
  );
}
