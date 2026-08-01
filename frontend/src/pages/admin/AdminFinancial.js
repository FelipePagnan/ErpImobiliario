import React, { useState, useEffect } from 'react';
import { financeiroApi } from '../../services/api';
import '../admin/AdminPages.css';

const formatPrice = (v) => v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : 'R$ 0,00';
const formatDate = (d) => d ? new Date(d).toLocaleDateString('pt-BR') : '—';

export default function AdminFinancial() {
  const [resumo, setResumo] = useState(null);
  const [lancamentos, setLancamentos] = useState([]);
  const [comissoes, setComissoes] = useState([]);
  const [tab, setTab] = useState('resumo');
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({ descricao: '', tipo: 1, categoria: 1, valor: '', dataVencimento: '' });

  useEffect(() => { fetchAll(); }, []);

  const fetchAll = async () => {
    try {
      const [r, l, c] = await Promise.all([financeiroApi.resumo(), financeiroApi.lancamentos(), financeiroApi.comissoes()]);
      setResumo(r.data); setLancamentos(l.data); setComissoes(c.data);
    } catch (err) { console.error(err); }
    finally { setLoading(false); }
  };

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      await financeiroApi.criarLancamento({ ...form, valor: parseFloat(form.valor) });
      setShowForm(false); setForm({ descricao: '', tipo: 1, categoria: 1, valor: '', dataVencimento: '' });
      fetchAll();
    } catch (err) { alert('Erro ao criar lançamento'); }
  };

  const handlePagar = async (id) => {
    try { await financeiroApi.pagarLancamento(id); fetchAll(); } catch { alert('Erro'); }
  };

  const handlePagarComissao = async (id) => {
    try { await financeiroApi.pagarComissao(id); fetchAll(); } catch { alert('Erro'); }
  };

  if (loading) return <div className="admin-page"><div className="container"><div className="loading">Carregando...</div></div></div>;

  return (
    <div className="admin-page">
      <div className="container">
        <h1 className="page-title">Financeiro</h1>

        <div className="tab-bar">
          <button className={`tab-btn ${tab==='resumo'?'active':''}`} onClick={()=>setTab('resumo')}>Resumo</button>
          <button className={`tab-btn ${tab==='lancamentos'?'active':''}`} onClick={()=>setTab('lancamentos')}>Lançamentos</button>
          <button className={`tab-btn ${tab==='comissoes'?'active':''}`} onClick={()=>setTab('comissoes')}>Comissões</button>
        </div>

        {tab === 'resumo' && resumo && (
          <div className="dashboard-cards" style={{marginTop:'1.5rem'}}>
            <div className="dash-card dash-card-success"><span className="dash-card-icon">💰</span><div className="dash-card-info"><span className="dash-card-number">{formatPrice(resumo.receitaTotal)}</span><span className="dash-card-label">Receita Realizada</span></div></div>
            <div className="dash-card" style={{borderLeftColor:'#c53030'}}><span className="dash-card-icon">📉</span><div className="dash-card-info"><span className="dash-card-number">{formatPrice(resumo.despesaTotal)}</span><span className="dash-card-label">Despesa Realizada</span></div></div>
            <div className="dash-card dash-card-primary"><span className="dash-card-icon">📊</span><div className="dash-card-info"><span className="dash-card-number" style={{color: resumo.saldo >= 0 ? '#2f855a' : '#c53030'}}>{formatPrice(resumo.saldo)}</span><span className="dash-card-label">Saldo</span></div></div>
            <div className="dash-card dash-card-warning"><span className="dash-card-icon">⏳</span><div className="dash-card-info"><span className="dash-card-number">{formatPrice(resumo.receitaPendente)}</span><span className="dash-card-label">A Receber</span></div></div>
            <div className="dash-card dash-card-info"><span className="dash-card-icon">📋</span><div className="dash-card-info"><span className="dash-card-number">{formatPrice(resumo.comissoesPendentes)}</span><span className="dash-card-label">Comissões Pendentes</span></div></div>
            <div className="dash-card" style={{borderLeftColor: resumo.lancamentosVencidos > 0 ? '#c53030' : '#e2e5ea'}}><span className="dash-card-icon">🔴</span><div className="dash-card-info"><span className="dash-card-number">{resumo.lancamentosVencidos}</span><span className="dash-card-label">Vencidos</span></div></div>
          </div>
        )}

        {tab === 'lancamentos' && (
          <>
            <div style={{display:'flex',justifyContent:'flex-end',margin:'1rem 0'}}>
              <button className="btn btn-primary" onClick={()=>setShowForm(!showForm)}>{showForm ? 'Cancelar' : '+ Novo Lançamento'}</button>
            </div>
            {showForm && (
              <form className="admin-form fade-in" onSubmit={handleCreate}>
                <div className="form-row">
                  <div className="form-group" style={{flex:2}}><label className="form-label">Descrição</label>
                    <input className="form-input" value={form.descricao} onChange={e=>setForm({...form,descricao:e.target.value})} required /></div>
                  <div className="form-group"><label className="form-label">Tipo</label>
                    <select className="form-select" value={form.tipo} onChange={e=>setForm({...form,tipo:parseInt(e.target.value)})}>
                      <option value={1}>Receita</option><option value={2}>Despesa</option></select></div>
                </div>
                <div className="form-row">
                  <div className="form-group"><label className="form-label">Categoria</label>
                    <select className="form-select" value={form.categoria} onChange={e=>setForm({...form,categoria:parseInt(e.target.value)})}>
                      <option value={1}>Aluguel</option><option value={2}>Comissão</option><option value={3}>Repasse</option>
                      <option value={4}>Condomínio</option><option value={5}>IPTU</option><option value={6}>Manutenção</option>
                      <option value={7}>Marketing</option><option value={8}>Administrativa</option><option value={9}>Venda</option>
                      <option value={10}>Outros</option></select></div>
                  <div className="form-group"><label className="form-label">Valor (R$)</label>
                    <input type="number" step="0.01" className="form-input" value={form.valor} onChange={e=>setForm({...form,valor:e.target.value})} required /></div>
                  <div className="form-group"><label className="form-label">Vencimento</label>
                    <input type="date" className="form-input" value={form.dataVencimento} onChange={e=>setForm({...form,dataVencimento:e.target.value})} required /></div>
                </div>
                <button type="submit" className="btn btn-accent">Criar Lançamento</button>
              </form>
            )}
            <div className="dashboard-table-wrapper">
              <table className="dashboard-table">
                <thead><tr><th>Descrição</th><th>Tipo</th><th>Categoria</th><th>Valor</th><th>Vencimento</th><th>Status</th><th>Ações</th></tr></thead>
                <tbody>
                  {lancamentos.length === 0 ? <tr><td colSpan="7" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhum lançamento</td></tr> :
                  lancamentos.map(l => (
                    <tr key={l.id}>
                      <td>{l.descricao}</td>
                      <td><span className="badge" style={{background: l.tipo==='Receita'?'#f0fff4':'#fff5f5',color:l.tipo==='Receita'?'#2f855a':'#c53030'}}>{l.tipo}</span></td>
                      <td>{l.categoria}</td>
                      <td style={{fontWeight:600}}>{formatPrice(l.valor)}</td>
                      <td>{formatDate(l.dataVencimento)}</td>
                      <td>{l.pago ? <span className="badge badge-disponivel">Pago</span> : <span className="badge badge-alugado">Pendente</span>}</td>
                      <td>{!l.pago && <button className="btn btn-outline btn-sm" onClick={()=>handlePagar(l.id)} style={{fontSize:'0.75rem'}}>Pagar</button>}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </>
        )}

        {tab === 'comissoes' && (
          <div className="dashboard-table-wrapper" style={{marginTop:'1.5rem'}}>
            <table className="dashboard-table">
              <thead><tr><th>Corretor</th><th>Base</th><th>%</th><th>Comissão</th><th>Data</th><th>Status</th><th>Ações</th></tr></thead>
              <tbody>
                {comissoes.length === 0 ? <tr><td colSpan="7" style={{textAlign:'center',padding:'2rem',color:'#8b95a5'}}>Nenhuma comissão</td></tr> :
                comissoes.map(c => (
                  <tr key={c.id}>
                    <td>{c.corretorNome}</td>
                    <td>{formatPrice(c.valorBase)}</td>
                    <td>{c.percentual}%</td>
                    <td style={{fontWeight:600}}>{formatPrice(c.valorComissao)}</td>
                    <td>{formatDate(c.dataCalculo)}</td>
                    <td>{c.pago ? <span className="badge badge-disponivel">Pago</span> : <span className="badge badge-alugado">Pendente</span>}</td>
                    <td>{!c.pago && <button className="btn btn-outline btn-sm" onClick={()=>handlePagarComissao(c.id)} style={{fontSize:'0.75rem'}}>Pagar</button>}</td>
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
