import React, { useState, useEffect } from 'react';
import '../admin/AdminPages.css';
import api from '../../services/api';

export default function AdminAudit() {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get('/auditoria?limite=200')
      .then(({ data }) => setLogs(data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const formatDate = (d) => new Date(d).toLocaleString('pt-BR');

  return (
    <div className="admin-page"><div className="container">
      <h1 className="page-title">Auditoria</h1>
      <p style={{color:'var(--color-text-muted)',fontSize:'0.875rem',marginBottom:'1.5rem'}}>Registro das operações realizadas no sistema.</p>

      {loading ? <div className="loading">Carregando...</div> : (
        <div className="dashboard-table-wrapper"><table className="dashboard-table">
          <thead><tr><th>Data/Hora</th><th>Ação</th><th>Entidade</th><th>Detalhes</th><th>Usuário</th></tr></thead>
          <tbody>
            {logs.length === 0 ? <tr><td colSpan="5" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhum registro de auditoria</td></tr> :
            logs.map(l=>(
              <tr key={l.id}>
                <td style={{whiteSpace:'nowrap',fontSize:'0.8rem'}}>{formatDate(l.criadoEm)}</td>
                <td><span className="badge" style={{background:'var(--color-info-bg)',color:'var(--color-info)'}}>{l.acao}</span></td>
                <td style={{fontWeight:500}}>{l.entidade}</td>
                <td style={{fontSize:'0.8rem',color:'var(--color-text-secondary)',maxWidth:300,overflow:'hidden',textOverflow:'ellipsis'}}>{l.detalhes || '—'}</td>
                <td style={{fontSize:'0.8rem'}}>{l.usuarioNome || 'Sistema'}<br/><span style={{color:'var(--color-text-muted)'}}>{l.usuarioEmail || ''}</span></td>
              </tr>
            ))}
          </tbody>
        </table></div>
      )}
    </div></div>
  );
}
