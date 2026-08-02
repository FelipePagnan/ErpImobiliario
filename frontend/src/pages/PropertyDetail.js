import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { imoveisApi, visitasApi, favoritosApi, clientesApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { useToast } from '../components/Toast';
import './PropertyDetail.css';

function formatPrice(v) { return v ? new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(v) : null; }
const placeholderImg = 'data:image/svg+xml,' + encodeURIComponent('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 500" fill="none"><rect width="800" height="500" fill="#e2e5ea"/><text x="400" y="250" text-anchor="middle" dy=".3em" fill="#8b95a5" font-family="sans-serif" font-size="20">Sem foto</text></svg>');

export default function PropertyDetail() {
  const { id } = useParams();
  const { usuario } = useAuth();
  const toast = useToast();
  const [imovel, setImovel] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showVisitaForm, setShowVisitaForm] = useState(false);
  const [visitaObs, setVisitaObs] = useState('');
  const [visitaData, setVisitaData] = useState('');
  const [visitaLoading, setVisitaLoading] = useState(false);
  const [fav, setFav] = useState(false);
  const [clienteId, setClienteId] = useState(null);
  const isCliente = usuario?.perfil === 'Cliente';

  useEffect(() => {
    imoveisApi.obterPorId(id).then(({ data }) => setImovel(data)).catch(console.error).finally(() => setLoading(false));
  }, [id]);

  useEffect(() => {
    if (!isCliente) return;
    const check = async () => {
      try {
        const { data: c } = await clientesApi.meuPerfil();
        setClienteId(c.id);
        const { data } = await favoritosApi.verificar(c.id, id);
        setFav(data.favorito);
      } catch {}
    };
    check();
  }, [isCliente, id]);

  const toggleFav = async () => {
    if (!clienteId) return;
    try {
      if (fav) { await favoritosApi.remover(clienteId, id); setFav(false); toast.info('Removido dos favoritos'); }
      else { await favoritosApi.adicionar(clienteId, id); setFav(true); toast.success('Adicionado aos favoritos!'); }
    } catch { toast.error('Erro ao atualizar favorito'); }
  };

  const handleVisita = async () => {
    if (!usuario) { toast.error('Faça login para solicitar uma visita.'); return; }
    setVisitaLoading(true);
    try {
      let cId = clienteId;
      if (!cId) { const { data: c } = await clientesApi.meuPerfil(); cId = c.id; setClienteId(cId); }
      await visitasApi.criar({ imovelId: id, clienteId: cId, observacoes: visitaObs || null, dataAgendada: visitaData || null });
      toast.success('Visita solicitada com sucesso!');
      setShowVisitaForm(false); setVisitaObs(''); setVisitaData('');
    } catch (err) {
      toast.error(err.response?.status === 404 ? 'Perfil de cliente não encontrado.' : 'Erro ao solicitar visita.');
    } finally { setVisitaLoading(false); }
  };

  const handleShare = (type) => {
    const url = window.location.href;
    const text = `${imovel.titulo} - ${imovel.precoVenda ? formatPrice(imovel.precoVenda) : imovel.precoLocacao ? formatPrice(imovel.precoLocacao)+'/mês' : 'Consulte'}`;
    if (type === 'whatsapp') { window.open(`https://wa.me/?text=${encodeURIComponent(text + '\n' + url)}`, '_blank'); }
    else if (type === 'copy') { navigator.clipboard.writeText(url).then(() => toast.success('Link copiado!')).catch(() => toast.error('Erro ao copiar')); }
  };

  if (loading) return <div className="container" style={{padding:'4rem 0'}}><div className="loading">Carregando...</div></div>;
  if (!imovel) return <div className="container" style={{padding:'4rem 0'}}><div className="empty-state">Imóvel não encontrado.</div></div>;

  return (
    <div className="detail-page"><div className="container">
      <Link to="/imoveis" className="back-link">← Voltar aos imóveis</Link>

      <div className="detail-header">
        <div>
          <span className={`badge badge-${imovel.finalidade==='Venda'?'venda':'locacao'}`}>{imovel.finalidade==='Locacao'?'Locação':imovel.finalidade==='VendaELocacao'?'Venda / Locação':imovel.finalidade}</span>
          <h1 className="detail-title">{imovel.titulo}</h1>
          {imovel.endereco && <p className="detail-location">{imovel.endereco.logradouro}, {imovel.endereco.numero}{imovel.endereco.complemento ? ` - ${imovel.endereco.complemento}` : ''} · {imovel.endereco.bairro}, {imovel.endereco.cidade} - {imovel.endereco.estado}</p>}
        </div>
        <div className="detail-pricing">
          {imovel.precoVenda && <div className="price-block"><span className="price-label">Venda</span><span className="price-value">{formatPrice(imovel.precoVenda)}</span></div>}
          {imovel.precoLocacao && <div className="price-block"><span className="price-label">Locação</span><span className="price-value">{formatPrice(imovel.precoLocacao)}/mês</span></div>}
        </div>
      </div>

      <div className="detail-image">
        <img src={imovel.fotoPrincipalUrl || placeholderImg} alt={imovel.titulo} />
        {/* Share + Fav buttons */}
        <div className="detail-actions">
          {isCliente && <button className={`action-btn ${fav?'fav-active':''}`} onClick={toggleFav} title={fav?'Remover favorito':'Favoritar'}>{fav?'❤️':'🤍'}</button>}
          <button className="action-btn" onClick={()=>handleShare('whatsapp')} title="Compartilhar no WhatsApp">📱</button>
          <button className="action-btn" onClick={()=>handleShare('copy')} title="Copiar link">🔗</button>
        </div>
      </div>

      <div className="detail-grid">
        <div className="detail-main">
          <section className="detail-section"><h2>Sobre o imóvel</h2><p>{imovel.descricao || 'Sem descrição disponível.'}</p></section>

          <section className="detail-section"><h2>Características</h2>
            <div className="features-grid">
              <div className="feature-item"><span className="feature-label">Tipo</span><span className="feature-value">{imovel.tipo}</span></div>
              <div className="feature-item"><span className="feature-label">Área total</span><span className="feature-value">{imovel.areaTotal} m²</span></div>
              {imovel.areaConstruida && <div className="feature-item"><span className="feature-label">Área construída</span><span className="feature-value">{imovel.areaConstruida} m²</span></div>}
              {imovel.dormitorios > 0 && <div className="feature-item"><span className="feature-label">Dormitórios</span><span className="feature-value">{imovel.dormitorios}</span></div>}
              {imovel.suites > 0 && <div className="feature-item"><span className="feature-label">Suítes</span><span className="feature-value">{imovel.suites}</span></div>}
              {imovel.banheiros > 0 && <div className="feature-item"><span className="feature-label">Banheiros</span><span className="feature-value">{imovel.banheiros}</span></div>}
              {imovel.vagasGaragem > 0 && <div className="feature-item"><span className="feature-label">Vagas</span><span className="feature-value">{imovel.vagasGaragem}</span></div>}
              {imovel.andar && <div className="feature-item"><span className="feature-label">Andar</span><span className="feature-value">{imovel.andar}º</span></div>}
              {imovel.mobiliado != null && <div className="feature-item"><span className="feature-label">Mobiliado</span><span className="feature-value">{imovel.mobiliado ? 'Sim' : 'Não'}</span></div>}
            </div>
          </section>

          {imovel.caracteristicas?.length > 0 && <section className="detail-section"><h2>Diferenciais</h2><div className="tags-list">{imovel.caracteristicas.map((c,i)=><span key={i} className="tag">{c}</span>)}</div></section>}
        </div>

        <aside className="detail-sidebar">
          <div className="sidebar-card"><h3>Custos</h3>
            <div className="cost-list">
              {imovel.valorCondominio && <div className="cost-row"><span>Condomínio</span><span>{formatPrice(imovel.valorCondominio)}</span></div>}
              {imovel.valorIPTU && <div className="cost-row"><span>IPTU (anual)</span><span>{formatPrice(imovel.valorIPTU)}</span></div>}
            </div>
          </div>

          {imovel.corretorNome && (
            <div className="sidebar-card"><h3>Corretor responsável</h3>
              <p className="broker-name">{imovel.corretorNome}</p>
              {imovel.corretorTelefone && <p className="broker-phone">{imovel.corretorTelefone}</p>}

              {!showVisitaForm ? (
                <button className="btn btn-accent" style={{width:'100%',marginTop:'1rem'}} onClick={()=>{
                  if(!usuario){toast.error('Faça login para solicitar.');return;} setShowVisitaForm(true);
                }}>Solicitar visita</button>
              ) : (
                <div className="visita-form fade-in" style={{marginTop:'1rem'}}>
                  <div className="form-group"><label className="form-label">Data preferida</label><input type="datetime-local" className="form-input" value={visitaData} onChange={e=>setVisitaData(e.target.value)} /></div>
                  <div className="form-group"><label className="form-label">Observações</label><textarea className="form-input" rows={2} placeholder="Ex: Manhã de sábado" value={visitaObs} onChange={e=>setVisitaObs(e.target.value)} /></div>
                  <div style={{display:'flex',gap:'0.5rem'}}>
                    <button className="btn btn-accent" style={{flex:1}} onClick={handleVisita} disabled={visitaLoading}>{visitaLoading?'Enviando...':'Confirmar'}</button>
                    <button className="btn btn-outline" onClick={()=>setShowVisitaForm(false)}>Cancelar</button>
                  </div>
                </div>
              )}
            </div>
          )}

          <div className="sidebar-card">
            <h3>Compartilhar</h3>
            <div style={{display:'flex',gap:'0.5rem'}}>
              <button className="btn btn-outline" style={{flex:1,fontSize:'0.8rem'}} onClick={()=>handleShare('whatsapp')}>📱 WhatsApp</button>
              <button className="btn btn-outline" style={{flex:1,fontSize:'0.8rem'}} onClick={()=>handleShare('copy')}>🔗 Copiar link</button>
            </div>
          </div>

          <div className="sidebar-card"><p className="property-code">Código: {imovel.codigo}</p></div>
        </aside>
      </div>
    </div></div>
  );
}
