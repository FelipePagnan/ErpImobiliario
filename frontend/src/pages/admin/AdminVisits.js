import React, { useState, useEffect } from 'react';
import { visitasApi } from '../../services/api';
import '../admin/AdminPages.css';

const formatDate = (d) => d ? new Date(d).toLocaleDateString('pt-BR') : '—';
const formatDateTime = (d) => d ? new Date(d).toLocaleString('pt-BR') : '—';
const statusColors = { Solicitada: '#c05621', Agendada: '#2b6cb0', Realizada: '#2f855a', Cancelada: '#c53030', NaoCompareceu: '#718096' };
const statusLabels = { Solicitada: 'Solicitada', Agendada: 'Agendada', Realizada: 'Realizada', Cancelada: 'Cancelada', NaoCompareceu: 'Não Compareceu' };

export default function AdminVisits() {
  const [visitas, setVisitas] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => { fetchVisitas(); }, []);

  const fetchVisitas = async () => {
    try { const { data } = await visitasApi.listar(); setVisitas(data); }
    catch (err) { console.error(err); }
    finally { setLoading(false); }
  };

  const handleStatus = async (id, status) => {
    try { await visitasApi.atualizar(id, { status }); fetchVisitas(); }
    catch { alert('Erro ao atualizar'); }
  };

  const handleCancelar = async (id) => {
    if (!window.confirm('Cancelar esta visita?')) return;
    try { await visitasApi.cancelar(id); fetchVisitas(); }
    catch { alert('Erro'); }
  };

  return (
    <div className="admin-page">
      <div className="container">
        <h1 className="page-title">Visitas</h1>

        {loading ? <div className="loading">Carregando...</div> : (
          <div className="dashboard-table-wrapper">
            <table className="dashboard-table">
              <thead><tr><th>Imóvel</th><th>Cliente</th><th>Telefone</th><th>Corretor</th><th>Solicitação</th><th>Agendada</th><th>Status</th><th>Ações</th></tr></thead>
              <tbody>
                {visitas.length === 0 ? <tr><td colSpan="8" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhuma visita solicitada</td></tr> :
                visitas.map(v => (
                  <tr key={v.id}>
                    <td>{v.imovelTitulo}<br/><code style={{fontSize:'0.7rem'}}>{v.imovelCodigo}</code></td>
                    <td>{v.clienteNome}</td>
                    <td>{v.clienteTelefone || '—'}</td>
                    <td>{v.corretorNome || '—'}</td>
                    <td>{formatDateTime(v.dataSolicitacao)}</td>
                    <td>{formatDateTime(v.dataAgendada)}</td>
                    <td><span className="badge" style={{background:`${statusColors[v.status]}20`,color:statusColors[v.status]}}>{statusLabels[v.status] || v.status}</span></td>
                    <td>
                      <div style={{display:'flex',gap:'0.25rem',flexWrap:'wrap'}}>
                        {v.status === 'Solicitada' && <button className="btn btn-primary btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleStatus(v.id, 2)}>Agendar</button>}
                        {v.status === 'Agendada' && <button className="btn btn-accent btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleStatus(v.id, 3)}>Realizada</button>}
                        {(v.status === 'Solicitada' || v.status === 'Agendada') && <button className="btn btn-outline btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleCancelar(v.id)}>Cancelar</button>}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
