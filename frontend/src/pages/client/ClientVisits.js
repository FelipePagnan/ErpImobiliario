import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { visitasApi, clientesApi } from '../../services/api';

const statusLabels = { Solicitada:'Solicitada', Agendada:'Agendada', Realizada:'Realizada', Cancelada:'Cancelada', NaoCompareceu:'Não Compareceu' };
const statusColors = { Solicitada:'#c05621', Agendada:'#2b6cb0', Realizada:'#2f855a', Cancelada:'#c53030', NaoCompareceu:'#718096' };
const formatDate = (d) => d ? new Date(d).toLocaleString('pt-BR') : '—';

export default function ClientVisits() {
  const [visitas, setVisitas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [clienteId, setClienteId] = useState(null);

  useEffect(() => {
    const load = async () => {
      try {
        const { data: cliente } = await clientesApi.meuPerfil();
        setClienteId(cliente.id);
        const { data } = await visitasApi.porCliente(cliente.id);
        setVisitas(data);
      } catch (err) { console.error(err); }
      finally { setLoading(false); }
    };
    load();
  }, []);

  const handleCancelar = async (id) => {
    if (!window.confirm('Cancelar esta visita?')) return;
    try { await visitasApi.cancelar(id); const { data } = await visitasApi.porCliente(clienteId); setVisitas(data); }
    catch { alert('Erro ao cancelar'); }
  };

  return (
    <div style={{padding:'2rem 0'}}><div className="container">
      <h1 className="page-title">Minhas Visitas</h1>

      {loading ? <div className="loading">Carregando...</div> :
       visitas.length === 0 ? (
        <div className="empty-state">
          <p>Você ainda não solicitou nenhuma visita.</p>
          <Link to="/imoveis" className="btn btn-primary" style={{marginTop:'1rem'}}>Explorar imóveis</Link>
        </div>
      ) : (
        <div style={{display:'flex',flexDirection:'column',gap:'1rem'}}>
          {visitas.map(v => (
            <div key={v.id} className="sidebar-card" style={{display:'flex',justifyContent:'space-between',alignItems:'center',flexWrap:'wrap',gap:'1rem'}}>
              <div>
                <Link to={`/imoveis/${v.imovelId}`} style={{fontWeight:600,fontSize:'1rem'}}>{v.imovelTitulo}</Link>
                <p style={{fontSize:'0.8rem',color:'var(--color-text-muted)',margin:'0.25rem 0'}}>Código: {v.imovelCodigo} · Corretor: {v.corretorNome || 'A definir'}</p>
                {v.observacoes && <p style={{fontSize:'0.85rem',color:'var(--color-text-secondary)'}}>"{v.observacoes}"</p>}
              </div>
              <div style={{textAlign:'right',display:'flex',flexDirection:'column',alignItems:'flex-end',gap:'0.5rem'}}>
                <span className="badge" style={{background:`${statusColors[v.status]}20`,color:statusColors[v.status]}}>{statusLabels[v.status]||v.status}</span>
                <span style={{fontSize:'0.75rem',color:'var(--color-text-muted)'}}>Solicitada: {formatDate(v.dataSolicitacao)}</span>
                {v.dataAgendada && <span style={{fontSize:'0.75rem',color:'var(--color-primary)',fontWeight:500}}>Agendada: {formatDate(v.dataAgendada)}</span>}
                {(v.status === 'Solicitada' || v.status === 'Agendada') && (
                  <button className="btn btn-outline btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleCancelar(v.id)}>Cancelar</button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div></div>
  );
}
