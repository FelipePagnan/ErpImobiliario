import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { imoveisApi, financeiroApi } from '../services/api';
import './Dashboard.css';

const formatPrice = (v) => v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : 'R$ 0,00';

export default function Dashboard() {
  const { usuario } = useAuth();
  const [dashboard, setDashboard] = useState(null);
  const [resumoFin, setResumoFin] = useState(null);
  const [loading, setLoading] = useState(true);
  const isManager = usuario && ['Administrador','Gerente'].includes(usuario.perfil);

  useEffect(() => {
    const load = async () => {
      try {
        const { data: d } = await imoveisApi.dashboard();
        setDashboard(d);
        if (isManager) {
          try { const { data: r } = await financeiroApi.resumo(); setResumoFin(r); } catch {}
        }
      } catch(e) { console.error(e); }
      finally { setLoading(false); }
    };
    load();
  }, []); // eslint-disable-line

  if (loading) return <div className="dashboard-page"><div className="container"><div className="loading">Carregando painel...</div></div></div>;

  const totalImoveis = (dashboard?.imoveisDisponiveis||0) + (dashboard?.imoveisAlugados||0) + (dashboard?.imoveisVendidos||0);
  const pctDisp = totalImoveis ? (dashboard.imoveisDisponiveis/totalImoveis*100) : 0;
  const pctAlug = totalImoveis ? (dashboard.imoveisAlugados/totalImoveis*100) : 0;
  const pctVend = totalImoveis ? (dashboard.imoveisVendidos/totalImoveis*100) : 0;

  return (
    <div className="dashboard-page"><div className="container">
      <div className="dashboard-header">
        <div>
          <h1 className="dashboard-title">Painel Administrativo</h1>
          <p className="dashboard-welcome">Bem-vindo, <strong>{usuario?.nome}</strong><span className="dashboard-role">{usuario?.perfil}</span></p>
        </div>
        <Link to="/imoveis" className="btn btn-outline">Ver portal</Link>
      </div>

      {dashboard && (<>
        <div className="dashboard-cards">
          <div className="dash-card dash-card-primary"><span className="dash-card-icon">🏠</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.imoveisDisponiveis}</span><span className="dash-card-label">Disponíveis</span></div></div>
          <div className="dash-card dash-card-warning"><span className="dash-card-icon">🔑</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.imoveisAlugados}</span><span className="dash-card-label">Alugados</span></div></div>
          <div className="dash-card dash-card-success"><span className="dash-card-icon">✅</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.imoveisVendidos}</span><span className="dash-card-label">Vendidos</span></div></div>
          <div className="dash-card dash-card-info"><span className="dash-card-icon">👥</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.totalClientes}</span><span className="dash-card-label">Clientes</span></div></div>
          <div className="dash-card dash-card-accent"><span className="dash-card-icon">🤝</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.totalCorretores}</span><span className="dash-card-label">Corretores</span></div></div>
          <div className="dash-card"><span className="dash-card-icon">📋</span><div className="dash-card-info"><span className="dash-card-number">{dashboard.totalProprietarios}</span><span className="dash-card-label">Proprietários</span></div></div>
        </div>

        {/* Gráfico de barras simples */}
        <div className="chart-row">
          <div className="chart-card">
            <h3>Distribuição de Imóveis</h3>
            <div className="bar-chart">
              <div className="bar-item"><div className="bar-label">Disponíveis</div><div className="bar-track"><div className="bar-fill" style={{width:`${pctDisp}%`,background:'#2f855a'}}>{dashboard.imoveisDisponiveis}</div></div></div>
              <div className="bar-item"><div className="bar-label">Alugados</div><div className="bar-track"><div className="bar-fill" style={{width:`${pctAlug}%`,background:'#c05621'}}>{dashboard.imoveisAlugados}</div></div></div>
              <div className="bar-item"><div className="bar-label">Vendidos</div><div className="bar-track"><div className="bar-fill" style={{width:`${pctVend}%`,background:'#2b6cb0'}}>{dashboard.imoveisVendidos}</div></div></div>
            </div>
          </div>

          {isManager && resumoFin && (
            <div className="chart-card">
              <h3>Resumo Financeiro</h3>
              <div className="fin-summary">
                <div className="fin-row"><span>Receita Realizada</span><span style={{color:'#2f855a',fontWeight:700}}>{formatPrice(resumoFin.receitaTotal)}</span></div>
                <div className="fin-row"><span>Despesa Realizada</span><span style={{color:'#c53030',fontWeight:700}}>{formatPrice(resumoFin.despesaTotal)}</span></div>
                <div className="fin-row fin-saldo"><span>Saldo</span><span style={{color: resumoFin.saldo >= 0 ? '#2f855a' : '#c53030', fontWeight:700, fontSize:'1.25rem'}}>{formatPrice(resumoFin.saldo)}</span></div>
                <div className="fin-row"><span>A Receber</span><span style={{color:'#c05621'}}>{formatPrice(resumoFin.receitaPendente)}</span></div>
                <div className="fin-row"><span>Comissões Pendentes</span><span style={{color:'#c05621'}}>{formatPrice(resumoFin.comissoesPendentes)}</span></div>
                {resumoFin.lancamentosVencidos > 0 && <div className="fin-row" style={{color:'#c53030'}}><span>⚠ Vencidos</span><span style={{fontWeight:700}}>{resumoFin.lancamentosVencidos}</span></div>}
              </div>
            </div>
          )}
        </div>

        <section className="dashboard-section">
          <h2>Imóveis recentes</h2>
          {dashboard.imoveisRecentes?.length > 0 ? (
            <div className="dashboard-table-wrapper"><table className="dashboard-table">
              <thead><tr><th>Foto</th><th>Código</th><th>Título</th><th>Tipo</th><th>Status</th><th>Preço</th><th>Corretor</th></tr></thead>
              <tbody>{dashboard.imoveisRecentes.map(im=>(
                <tr key={im.id}>
                  <td><img src={im.fotoPrincipalUrl||'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 40 30"><rect fill="%23e2e5ea" width="40" height="30"/></svg>'} alt="" style={{width:50,height:34,objectFit:'cover',borderRadius:4}} /></td>
                  <td><code>{im.codigo}</code></td>
                  <td><Link to={`/imoveis/${im.id}`} className="table-link">{im.titulo}</Link></td>
                  <td>{im.tipo}</td>
                  <td><span className="badge badge-disponivel">{im.status}</span></td>
                  <td>{im.precoVenda?formatPrice(im.precoVenda):im.precoLocacao?formatPrice(im.precoLocacao)+'/mês':'—'}</td>
                  <td>{im.corretorNome||'—'}</td>
                </tr>
              ))}</tbody>
            </table></div>
          ) : <p className="empty-state">Nenhum imóvel cadastrado.</p>}
        </section>
      </>)}
    </div></div>
  );
}
