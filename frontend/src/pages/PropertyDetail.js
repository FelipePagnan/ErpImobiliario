import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { imoveisApi } from '../services/api';
import './PropertyDetail.css';

function formatPrice(value) {
  if (!value) return null;
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

const placeholderImg = 'data:image/svg+xml,' + encodeURIComponent(
  '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 500" fill="none"><rect width="800" height="500" fill="#e2e5ea"/><text x="400" y="250" text-anchor="middle" dy=".3em" fill="#8b95a5" font-family="sans-serif" font-size="20">Sem foto disponível</text></svg>'
);

export default function PropertyDetail() {
  const { id } = useParams();
  const [imovel, setImovel] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    imoveisApi.obterPorId(id)
      .then(({ data }) => setImovel(data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className="container" style={{ padding: '4rem 0' }}><div className="loading">Carregando...</div></div>;
  if (!imovel) return <div className="container" style={{ padding: '4rem 0' }}><div className="empty-state">Imóvel não encontrado.</div></div>;

  return (
    <div className="detail-page">
      <div className="container">
        <Link to="/imoveis" className="back-link">← Voltar aos imóveis</Link>

        <div className="detail-header">
          <div>
            <span className={`badge badge-${imovel.finalidade === 'Venda' ? 'venda' : 'locacao'}`}>
              {imovel.finalidade === 'Locacao' ? 'Locação' : imovel.finalidade === 'VendaELocacao' ? 'Venda / Locação' : imovel.finalidade}
            </span>
            <h1 className="detail-title">{imovel.titulo}</h1>
            {imovel.endereco && (
              <p className="detail-location">
                {imovel.endereco.logradouro}, {imovel.endereco.numero}
                {imovel.endereco.complemento ? ` - ${imovel.endereco.complemento}` : ''}
                {' · '}{imovel.endereco.bairro}, {imovel.endereco.cidade} - {imovel.endereco.estado}
              </p>
            )}
          </div>
          <div className="detail-pricing">
            {imovel.precoVenda && (
              <div className="price-block">
                <span className="price-label">Venda</span>
                <span className="price-value">{formatPrice(imovel.precoVenda)}</span>
              </div>
            )}
            {imovel.precoLocacao && (
              <div className="price-block">
                <span className="price-label">Locação</span>
                <span className="price-value">{formatPrice(imovel.precoLocacao)}/mês</span>
              </div>
            )}
          </div>
        </div>

        <div className="detail-image">
          <img src={imovel.fotoPrincipalUrl || placeholderImg} alt={imovel.titulo} />
        </div>

        <div className="detail-grid">
          <div className="detail-main">
            <section className="detail-section">
              <h2>Sobre o imóvel</h2>
              <p>{imovel.descricao || 'Sem descrição disponível.'}</p>
            </section>

            <section className="detail-section">
              <h2>Características</h2>
              <div className="features-grid">
                <div className="feature-item">
                  <span className="feature-label">Tipo</span>
                  <span className="feature-value">{imovel.tipo}</span>
                </div>
                <div className="feature-item">
                  <span className="feature-label">Área total</span>
                  <span className="feature-value">{imovel.areaTotal} m²</span>
                </div>
                {imovel.areaConstruida && (
                  <div className="feature-item">
                    <span className="feature-label">Área construída</span>
                    <span className="feature-value">{imovel.areaConstruida} m²</span>
                  </div>
                )}
                {imovel.dormitorios > 0 && (
                  <div className="feature-item">
                    <span className="feature-label">Dormitórios</span>
                    <span className="feature-value">{imovel.dormitorios}</span>
                  </div>
                )}
                {imovel.suites > 0 && (
                  <div className="feature-item">
                    <span className="feature-label">Suítes</span>
                    <span className="feature-value">{imovel.suites}</span>
                  </div>
                )}
                {imovel.banheiros > 0 && (
                  <div className="feature-item">
                    <span className="feature-label">Banheiros</span>
                    <span className="feature-value">{imovel.banheiros}</span>
                  </div>
                )}
                {imovel.vagasGaragem > 0 && (
                  <div className="feature-item">
                    <span className="feature-label">Vagas de garagem</span>
                    <span className="feature-value">{imovel.vagasGaragem}</span>
                  </div>
                )}
                {imovel.andar && (
                  <div className="feature-item">
                    <span className="feature-label">Andar</span>
                    <span className="feature-value">{imovel.andar}º</span>
                  </div>
                )}
                {imovel.mobiliado !== null && imovel.mobiliado !== undefined && (
                  <div className="feature-item">
                    <span className="feature-label">Mobiliado</span>
                    <span className="feature-value">{imovel.mobiliado ? 'Sim' : 'Não'}</span>
                  </div>
                )}
              </div>
            </section>

            {imovel.caracteristicas && imovel.caracteristicas.length > 0 && (
              <section className="detail-section">
                <h2>Diferenciais</h2>
                <div className="tags-list">
                  {imovel.caracteristicas.map((c, i) => (
                    <span key={i} className="tag">{c}</span>
                  ))}
                </div>
              </section>
            )}
          </div>

          <aside className="detail-sidebar">
            <div className="sidebar-card">
              <h3>Custos mensais</h3>
              <div className="cost-list">
                {imovel.valorCondominio && (
                  <div className="cost-row">
                    <span>Condomínio</span>
                    <span>{formatPrice(imovel.valorCondominio)}</span>
                  </div>
                )}
                {imovel.valorIPTU && (
                  <div className="cost-row">
                    <span>IPTU (anual)</span>
                    <span>{formatPrice(imovel.valorIPTU)}</span>
                  </div>
                )}
              </div>
            </div>

            {imovel.corretorNome && (
              <div className="sidebar-card">
                <h3>Corretor responsável</h3>
                <p className="broker-name">{imovel.corretorNome}</p>
                {imovel.corretorTelefone && (
                  <p className="broker-phone">{imovel.corretorTelefone}</p>
                )}
                <button className="btn btn-accent" style={{ width: '100%', marginTop: '1rem' }}>
                  Solicitar visita
                </button>
              </div>
            )}

            <div className="sidebar-card">
              <p className="property-code">Código: {imovel.codigo}</p>
            </div>
          </aside>
        </div>
      </div>
    </div>
  );
}
