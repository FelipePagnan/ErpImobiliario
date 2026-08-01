import React, { useState, useEffect } from 'react';
import { contratosApi } from '../../services/api';
import '../admin/AdminPages.css';

const formatPrice = (v) => v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : '—';
const formatDate = (d) => d ? new Date(d).toLocaleDateString('pt-BR') : '—';

const statusColors = { Ativo: '#2f855a', Encerrado: '#718096', Rescindido: '#c53030', EmAnalise: '#c05621', Renovado: '#2b6cb0' };

export default function AdminContracts() {
  const [contratos, setContratos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ imovelId: '', clienteId: '', tipo: 2, dataInicio: '', dataFim: '', valorTotal: '', valorMensal: '' });

  useEffect(() => { fetchContratos(); }, []);

  const fetchContratos = async () => {
    try { const { data } = await contratosApi.listar(); setContratos(data); }
    catch (err) { console.error(err); }
    finally { setLoading(false); }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      await contratosApi.criar({ ...form, valorTotal: parseFloat(form.valorTotal), valorMensal: form.valorMensal ? parseFloat(form.valorMensal) : null });
      setShowForm(false);
      setForm({ imovelId: '', clienteId: '', tipo: 2, dataInicio: '', dataFim: '', valorTotal: '', valorMensal: '' });
      fetchContratos();
    } catch (err) { alert(err.response?.data?.mensagem || 'Erro ao criar contrato'); }
  };

  const handleRescindir = async (id) => {
    if (!window.confirm('Confirma a rescisão deste contrato?')) return;
    const motivo = prompt('Motivo da rescisão (opcional):');
    try { await contratosApi.rescindir(id, motivo); fetchContratos(); }
    catch (err) { alert('Erro ao rescindir'); }
  };

  return (
    <div className="admin-page">
      <div className="container">
        <div className="admin-header">
          <h1 className="page-title">Contratos</h1>
          <button className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
            {showForm ? 'Cancelar' : '+ Novo Contrato'}
          </button>
        </div>

        {showForm && (
          <form className="admin-form fade-in" onSubmit={handleCreate}>
            <div className="form-row">
              <div className="form-group"><label className="form-label">ID do Imóvel</label>
                <input className="form-input" value={form.imovelId} onChange={e => setForm({...form, imovelId: e.target.value})} required /></div>
              <div className="form-group"><label className="form-label">ID do Cliente</label>
                <input className="form-input" value={form.clienteId} onChange={e => setForm({...form, clienteId: e.target.value})} required /></div>
            </div>
            <div className="form-row">
              <div className="form-group"><label className="form-label">Tipo</label>
                <select className="form-select" value={form.tipo} onChange={e => setForm({...form, tipo: parseInt(e.target.value)})}>
                  <option value={1}>Venda</option><option value={2}>Locação</option></select></div>
              <div className="form-group"><label className="form-label">Data Início</label>
                <input type="date" className="form-input" value={form.dataInicio} onChange={e => setForm({...form, dataInicio: e.target.value})} required /></div>
              <div className="form-group"><label className="form-label">Data Fim</label>
                <input type="date" className="form-input" value={form.dataFim} onChange={e => setForm({...form, dataFim: e.target.value})} /></div>
            </div>
            <div className="form-row">
              <div className="form-group"><label className="form-label">Valor Total</label>
                <input type="number" step="0.01" className="form-input" value={form.valorTotal} onChange={e => setForm({...form, valorTotal: e.target.value})} required /></div>
              <div className="form-group"><label className="form-label">Valor Mensal</label>
                <input type="number" step="0.01" className="form-input" value={form.valorMensal} onChange={e => setForm({...form, valorMensal: e.target.value})} /></div>
            </div>
            <button type="submit" className="btn btn-accent">Criar Contrato</button>
          </form>
        )}

        {loading ? <div className="loading">Carregando...</div> : (
          <div className="dashboard-table-wrapper">
            <table className="dashboard-table">
              <thead><tr><th>Código</th><th>Imóvel</th><th>Cliente</th><th>Tipo</th><th>Status</th><th>Início</th><th>Fim</th><th>Valor</th><th>Ações</th></tr></thead>
              <tbody>
                {contratos.length === 0 ? <tr><td colSpan="9" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhum contrato cadastrado</td></tr> :
                contratos.map(c => (
                  <tr key={c.id}>
                    <td><code>{c.codigo}</code></td>
                    <td>{c.imovelTitulo}</td>
                    <td>{c.clienteNome}</td>
                    <td>{c.tipo === 'Locacao' ? 'Locação' : c.tipo}</td>
                    <td><span className="badge" style={{background: `${statusColors[c.status]}20`, color: statusColors[c.status]}}>{c.status}</span></td>
                    <td>{formatDate(c.dataInicio)}</td>
                    <td>{formatDate(c.dataFim)}</td>
                    <td>{c.valorMensal ? formatPrice(c.valorMensal)+'/mês' : formatPrice(c.valorTotal)}</td>
                    <td>
                      {c.status === 'Ativo' && (
                        <button className="btn btn-outline btn-sm" onClick={() => handleRescindir(c.id)} style={{fontSize:'0.75rem'}}>Rescindir</button>
                      )}
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
