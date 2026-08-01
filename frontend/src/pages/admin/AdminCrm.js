import React, { useState, useEffect } from 'react';
import { crmApi } from '../../services/api';
import '../admin/AdminPages.css';

const formatPrice = (v) => v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : '—';
const formatDate = (d) => d ? new Date(d).toLocaleDateString('pt-BR') : 'Nunca';
const tipoLabels = { 1:'Casa', 2:'Apartamento', 3:'Cobertura', 5:'Kitnet', 6:'Sobrado', 7:'Terreno', 11:'Sala Comercial' };
const finalidadeLabels = { 1:'Comprar', 2:'Alugar' };

export default function AdminCrm() {
  const [interessados, setInteressados] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ nome:'', email:'', telefone:'', cidadeDesejada:'', tipoImovelDesejado:'', finalidadeDesejada:'', orcamentoMaximo:'', observacoes:'' });

  useEffect(() => { fetchData(); }, []);

  const fetchData = async () => {
    try { const { data } = await crmApi.listar(); setInteressados(data); }
    catch (err) { console.error(err); }
    finally { setLoading(false); }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      const payload = { ...form,
        tipoImovelDesejado: form.tipoImovelDesejado ? parseInt(form.tipoImovelDesejado) : null,
        finalidadeDesejada: form.finalidadeDesejada ? parseInt(form.finalidadeDesejada) : null,
        orcamentoMaximo: form.orcamentoMaximo ? parseFloat(form.orcamentoMaximo) : null
      };
      await crmApi.criar(payload);
      setShowForm(false);
      setForm({ nome:'', email:'', telefone:'', cidadeDesejada:'', tipoImovelDesejado:'', finalidadeDesejada:'', orcamentoMaximo:'', observacoes:'' });
      fetchData();
    } catch { alert('Erro ao cadastrar'); }
  };

  const handleContato = async (id) => {
    const descricao = prompt('Descreva o contato realizado:');
    if (!descricao) return;
    const tipo = prompt('Tipo de contato (Telefone, Email, Presencial):', 'Telefone');
    try { await crmApi.registrarContato(id, { tipo: tipo || 'Telefone', descricao }); fetchData(); }
    catch { alert('Erro'); }
  };

  const handleRemover = async (id) => {
    if (!window.confirm('Remover este interessado?')) return;
    try { await crmApi.remover(id); fetchData(); }
    catch { alert('Erro'); }
  };

  return (
    <div className="admin-page">
      <div className="container">
        <div className="admin-header">
          <h1 className="page-title">CRM — Interessados</h1>
          <button className="btn btn-primary" onClick={()=>setShowForm(!showForm)}>{showForm ? 'Cancelar' : '+ Novo Interessado'}</button>
        </div>

        {showForm && (
          <form className="admin-form fade-in" onSubmit={handleCreate}>
            <div className="form-row">
              <div className="form-group"><label className="form-label">Nome</label>
                <input className="form-input" value={form.nome} onChange={e=>setForm({...form,nome:e.target.value})} required /></div>
              <div className="form-group"><label className="form-label">E-mail</label>
                <input type="email" className="form-input" value={form.email} onChange={e=>setForm({...form,email:e.target.value})} /></div>
              <div className="form-group"><label className="form-label">Telefone</label>
                <input className="form-input" value={form.telefone} onChange={e=>setForm({...form,telefone:e.target.value})} /></div>
            </div>
            <div className="form-row">
              <div className="form-group"><label className="form-label">Cidade desejada</label>
                <input className="form-input" value={form.cidadeDesejada} onChange={e=>setForm({...form,cidadeDesejada:e.target.value})} /></div>
              <div className="form-group"><label className="form-label">Tipo de imóvel</label>
                <select className="form-select" value={form.tipoImovelDesejado} onChange={e=>setForm({...form,tipoImovelDesejado:e.target.value})}>
                  <option value="">Qualquer</option><option value="1">Casa</option><option value="2">Apartamento</option>
                  <option value="5">Kitnet</option><option value="6">Sobrado</option><option value="7">Terreno</option></select></div>
              <div className="form-group"><label className="form-label">Finalidade</label>
                <select className="form-select" value={form.finalidadeDesejada} onChange={e=>setForm({...form,finalidadeDesejada:e.target.value})}>
                  <option value="">Qualquer</option><option value="1">Comprar</option><option value="2">Alugar</option></select></div>
              <div className="form-group"><label className="form-label">Orçamento máximo</label>
                <input type="number" step="0.01" className="form-input" value={form.orcamentoMaximo} onChange={e=>setForm({...form,orcamentoMaximo:e.target.value})} /></div>
            </div>
            <div className="form-group"><label className="form-label">Observações</label>
              <textarea className="form-input" rows={2} value={form.observacoes} onChange={e=>setForm({...form,observacoes:e.target.value})} /></div>
            <button type="submit" className="btn btn-accent">Cadastrar</button>
          </form>
        )}

        {loading ? <div className="loading">Carregando...</div> : (
          <div className="dashboard-table-wrapper">
            <table className="dashboard-table">
              <thead><tr><th>Nome</th><th>Contato</th><th>Busca</th><th>Orçamento</th><th>Último Contato</th><th>Compatíveis</th><th>Ações</th></tr></thead>
              <tbody>
                {interessados.length === 0 ? <tr><td colSpan="7" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhum interessado cadastrado</td></tr> :
                interessados.map(i => (
                  <tr key={i.id}>
                    <td style={{fontWeight:500}}>{i.nome}</td>
                    <td><div style={{fontSize:'0.8rem'}}>{i.email || '—'}<br/>{i.telefone || '—'}</div></td>
                    <td><div style={{fontSize:'0.8rem'}}>
                      {i.cidadeDesejada && <span>{i.cidadeDesejada}</span>}
                      {i.tipoImovelNome && <span> · {i.tipoImovelNome}</span>}
                      {i.finalidadeNome && <span> · {i.finalidadeNome}</span>}
                    </div></td>
                    <td>{formatPrice(i.orcamentoMaximo)}</td>
                    <td>{formatDate(i.ultimoContato)}</td>
                    <td><span className="badge" style={{background: i.imoveisCompativeis > 0 ? '#f0fff4' : '#fff5f5', color: i.imoveisCompativeis > 0 ? '#2f855a' : '#c53030'}}>{i.imoveisCompativeis}</span></td>
                    <td>
                      <div style={{display:'flex',gap:'0.25rem',flexWrap:'wrap'}}>
                        <button className="btn btn-primary btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleContato(i.id)}>Contato</button>
                        <button className="btn btn-outline btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleRemover(i.id)}>Remover</button>
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
