import React, { useState, useEffect } from 'react';
import { imoveisApi } from '../../services/api';
import '../admin/AdminPages.css';

const formatPrice = (v) => v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : '—';
const statusColors = { Disponivel: '#2f855a', Alugado: '#c05621', Vendido: '#2b6cb0', EmAnalise: '#e8a838', Indisponivel: '#718096', EmReforma: '#805ad5' };
const tipoOptions = [{v:1,l:'Casa'},{v:2,l:'Apartamento'},{v:3,l:'Cobertura'},{v:4,l:'Studio'},{v:5,l:'Kitnet'},{v:6,l:'Sobrado'},{v:7,l:'Terreno'},{v:8,l:'Chácara'},{v:9,l:'Fazenda'},{v:10,l:'Galpão'},{v:11,l:'Sala Comercial'},{v:12,l:'Loja'},{v:13,l:'Área Industrial'}];
const finalidadeOptions = [{v:1,l:'Venda'},{v:2,l:'Locação'},{v:3,l:'Venda e Locação'},{v:4,l:'Troca'}];

const emptyForm = {titulo:'',descricao:'',tipo:1,finalidade:1,precoVenda:'',precoLocacao:'',valorCondominio:'',valorIPTU:'',areaTotal:'',areaConstruida:'',dormitorios:'',suites:'',banheiros:'',vagasGaragem:'',andar:'',mobiliado:false,fotoPrincipalUrl:'',
  endereco:{logradouro:'',numero:'',complemento:'',bairro:'',cidade:'Maringá',estado:'PR',cep:''},proprietarioId:'',corretorId:''};

export default function AdminProperties() {
  const [imoveis, setImoveis] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState(null);
  const [form, setForm] = useState({...emptyForm});

  useEffect(() => { fetch(); }, []);
  const fetch = async () => { try { const {data} = await imoveisApi.listar(); setImoveis(data); } catch(e){console.error(e);} finally{setLoading(false);} };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const payload = {
      ...form, tipo: parseInt(form.tipo), finalidade: parseInt(form.finalidade),
      precoVenda: form.precoVenda ? parseFloat(form.precoVenda) : null,
      precoLocacao: form.precoLocacao ? parseFloat(form.precoLocacao) : null,
      valorCondominio: form.valorCondominio ? parseFloat(form.valorCondominio) : null,
      valorIPTU: form.valorIPTU ? parseFloat(form.valorIPTU) : null,
      areaTotal: parseFloat(form.areaTotal), areaConstruida: form.areaConstruida ? parseFloat(form.areaConstruida) : null,
      dormitorios: parseInt(form.dormitorios) || 0, suites: form.suites ? parseInt(form.suites) : null,
      banheiros: parseInt(form.banheiros) || 0, vagasGaragem: parseInt(form.vagasGaragem) || 0,
      andar: form.andar ? parseInt(form.andar) : null,
    };
    try {
      if (editId) { await imoveisApi.atualizar(editId, payload); }
      else { await imoveisApi.criar(payload); }
      setShowForm(false); setEditId(null); setForm({...emptyForm}); fetch();
    } catch(err) { alert(err.response?.data?.title || 'Erro ao salvar. Verifique IDs de proprietário e corretor.'); }
  };

  const handleEdit = (im) => {
    setForm({titulo:im.titulo,descricao:im.descricao||'',tipo:im.tipoId,finalidade:im.finalidadeId,
      precoVenda:im.precoVenda||'',precoLocacao:im.precoLocacao||'',valorCondominio:im.valorCondominio||'',valorIPTU:im.valorIPTU||'',
      areaTotal:im.areaTotal,areaConstruida:im.areaConstruida||'',dormitorios:im.dormitorios,suites:im.suites||'',
      banheiros:im.banheiros,vagasGaragem:im.vagasGaragem,andar:im.andar||'',mobiliado:im.mobiliado||false,
      fotoPrincipalUrl:im.fotoPrincipalUrl||'',endereco:{logradouro:'',numero:'',complemento:'',bairro:'',cidade:'',estado:'',cep:''},
      proprietarioId:'',corretorId:''});
    setEditId(im.id); setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Remover este imóvel?')) return;
    try { await imoveisApi.remover(id); fetch(); } catch { alert('Erro ao remover'); }
  };

  const set = (field, val) => setForm(f => ({...f, [field]: val}));
  const setEnd = (field, val) => setForm(f => ({...f, endereco: {...f.endereco, [field]: val}}));

  return (
    <div className="admin-page"><div className="container">
      <div className="admin-header">
        <h1 className="page-title">Gestão de Imóveis</h1>
        <button className="btn btn-primary" onClick={()=>{setShowForm(!showForm);setEditId(null);setForm({...emptyForm});}}>{showForm?'Cancelar':'+ Novo Imóvel'}</button>
      </div>

      {showForm && (
        <form className="admin-form fade-in" onSubmit={handleSubmit}>
          <h3 style={{marginBottom:'1rem',color:'var(--color-primary)'}}>{editId ? 'Editar Imóvel' : 'Novo Imóvel'}</h3>
          <div className="form-row">
            <div className="form-group" style={{flex:2}}><label className="form-label">Título *</label><input className="form-input" value={form.titulo} onChange={e=>set('titulo',e.target.value)} required /></div>
            <div className="form-group"><label className="form-label">Tipo *</label><select className="form-select" value={form.tipo} onChange={e=>set('tipo',e.target.value)}>{tipoOptions.map(t=><option key={t.v} value={t.v}>{t.l}</option>)}</select></div>
            <div className="form-group"><label className="form-label">Finalidade *</label><select className="form-select" value={form.finalidade} onChange={e=>set('finalidade',e.target.value)}>{finalidadeOptions.map(f=><option key={f.v} value={f.v}>{f.l}</option>)}</select></div>
          </div>
          <div className="form-group"><label className="form-label">Descrição</label><textarea className="form-input" rows={2} value={form.descricao} onChange={e=>set('descricao',e.target.value)} /></div>
          <div className="form-row">
            <div className="form-group"><label className="form-label">Preço Venda</label><input type="number" step="0.01" className="form-input" value={form.precoVenda} onChange={e=>set('precoVenda',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Preço Locação</label><input type="number" step="0.01" className="form-input" value={form.precoLocacao} onChange={e=>set('precoLocacao',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Condomínio</label><input type="number" step="0.01" className="form-input" value={form.valorCondominio} onChange={e=>set('valorCondominio',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">IPTU</label><input type="number" step="0.01" className="form-input" value={form.valorIPTU} onChange={e=>set('valorIPTU',e.target.value)} /></div>
          </div>
          <div className="form-row">
            <div className="form-group"><label className="form-label">Área Total (m²) *</label><input type="number" className="form-input" value={form.areaTotal} onChange={e=>set('areaTotal',e.target.value)} required /></div>
            <div className="form-group"><label className="form-label">Área Construída</label><input type="number" className="form-input" value={form.areaConstruida} onChange={e=>set('areaConstruida',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Quartos</label><input type="number" className="form-input" value={form.dormitorios} onChange={e=>set('dormitorios',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Suítes</label><input type="number" className="form-input" value={form.suites} onChange={e=>set('suites',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Banheiros</label><input type="number" className="form-input" value={form.banheiros} onChange={e=>set('banheiros',e.target.value)} /></div>
            <div className="form-group"><label className="form-label">Vagas</label><input type="number" className="form-input" value={form.vagasGaragem} onChange={e=>set('vagasGaragem',e.target.value)} /></div>
          </div>
          {!editId && (<>
          <h4 style={{margin:'0.5rem 0',color:'var(--color-text-secondary)',fontSize:'0.85rem'}}>Endereço</h4>
          <div className="form-row">
            <div className="form-group" style={{flex:2}}><label className="form-label">Logradouro *</label><input className="form-input" value={form.endereco.logradouro} onChange={e=>setEnd('logradouro',e.target.value)} required={!editId} /></div>
            <div className="form-group"><label className="form-label">Número *</label><input className="form-input" value={form.endereco.numero} onChange={e=>setEnd('numero',e.target.value)} required={!editId} /></div>
            <div className="form-group"><label className="form-label">Complemento</label><input className="form-input" value={form.endereco.complemento} onChange={e=>setEnd('complemento',e.target.value)} /></div>
          </div>
          <div className="form-row">
            <div className="form-group"><label className="form-label">Bairro *</label><input className="form-input" value={form.endereco.bairro} onChange={e=>setEnd('bairro',e.target.value)} required={!editId} /></div>
            <div className="form-group"><label className="form-label">Cidade *</label><input className="form-input" value={form.endereco.cidade} onChange={e=>setEnd('cidade',e.target.value)} required={!editId} /></div>
            <div className="form-group"><label className="form-label">Estado *</label><input className="form-input" maxLength={2} value={form.endereco.estado} onChange={e=>setEnd('estado',e.target.value.toUpperCase())} required={!editId} /></div>
            <div className="form-group"><label className="form-label">CEP *</label><input className="form-input" value={form.endereco.cep} onChange={e=>setEnd('cep',e.target.value)} required={!editId} /></div>
          </div>
          <div className="form-row">
            <div className="form-group"><label className="form-label">ID Proprietário *</label><input className="form-input" value={form.proprietarioId} onChange={e=>set('proprietarioId',e.target.value)} required={!editId} /></div>
            <div className="form-group"><label className="form-label">ID Corretor</label><input className="form-input" value={form.corretorId} onChange={e=>set('corretorId',e.target.value)} /></div>
          </div></>)}
          <div className="form-group"><label className="form-label">URL da Foto Principal</label><input className="form-input" value={form.fotoPrincipalUrl} onChange={e=>set('fotoPrincipalUrl',e.target.value)} placeholder="https://..." /></div>
          <button type="submit" className="btn btn-accent">{editId ? 'Salvar Alterações' : 'Cadastrar Imóvel'}</button>
        </form>
      )}

      {loading ? <div className="loading">Carregando...</div> : (
        <div className="dashboard-table-wrapper"><table className="dashboard-table">
          <thead><tr><th>Foto</th><th>Código</th><th>Título</th><th>Tipo</th><th>Finalidade</th><th>Status</th><th>Preço</th><th>Ações</th></tr></thead>
          <tbody>{imoveis.map(im=>(
            <tr key={im.id}>
              <td><img src={im.fotoPrincipalUrl || 'data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 40 30"><rect fill="%23e2e5ea" width="40" height="30"/></svg>'} alt="" style={{width:60,height:40,objectFit:'cover',borderRadius:4}} /></td>
              <td><code>{im.codigo}</code></td>
              <td style={{maxWidth:200,overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap'}}>{im.titulo}</td>
              <td>{im.tipo}</td>
              <td>{im.finalidade==='Locacao'?'Locação':im.finalidade}</td>
              <td><span className="badge" style={{background:`${statusColors[im.status]||'#718096'}20`,color:statusColors[im.status]||'#718096'}}>{im.status}</span></td>
              <td>{im.precoVenda?formatPrice(im.precoVenda):im.precoLocacao?formatPrice(im.precoLocacao)+'/mês':'—'}</td>
              <td><div style={{display:'flex',gap:'0.25rem'}}>
                <button className="btn btn-outline btn-sm" style={{fontSize:'0.7rem'}} onClick={()=>handleEdit(im)}>Editar</button>
                <button className="btn btn-outline btn-sm" style={{fontSize:'0.7rem',color:'#c53030',borderColor:'#c53030'}} onClick={()=>handleDelete(im.id)}>Remover</button>
              </div></td>
            </tr>
          ))}</tbody>
        </table></div>
      )}
    </div></div>
  );
}
