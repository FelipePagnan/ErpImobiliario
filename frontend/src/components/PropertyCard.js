import React from 'react';
import { Link } from 'react-router-dom';
import './PropertyCard.css';

const placeholderImg = 'data:image/svg+xml,' + encodeURIComponent(
  '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 260" fill="none"><rect width="400" height="260" fill="#e2e5ea"/><text x="200" y="130" text-anchor="middle" dy=".3em" fill="#8b95a5" font-family="sans-serif" font-size="16">Sem foto</text></svg>'
);

function formatPrice(value) {
  if (!value) return null;
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

function getFinalidadeBadgeClass(finalidade) {
  const map = {
    'Venda': 'badge-venda',
    'Locacao': 'badge-locacao',
    'VendaELocacao': 'badge-vendaElocacao',
    'Troca': 'badge-venda',
  };
  return map[finalidade] || 'badge-venda';
}

function getFinalidadeLabel(finalidade) {
  const map = {
    'Venda': 'Venda',
    'Locacao': 'Locação',
    'VendaELocacao': 'Venda / Locação',
    'Troca': 'Troca',
  };
  return map[finalidade] || finalidade;
}

export default function PropertyCard({ imovel }) {
  const preco = imovel.precoVenda
    ? formatPrice(imovel.precoVenda)
    : imovel.precoLocacao
      ? `${formatPrice(imovel.precoLocacao)}/mês`
      : 'Consulte';

  return (
    <Link to={`/imoveis/${imovel.id}`} className="property-card fade-in">
      <div className="property-card-img">
        <img src={imovel.fotoPrincipalUrl || placeholderImg} alt={imovel.titulo} />
        <span className={`badge ${getFinalidadeBadgeClass(imovel.finalidade)}`}>
          {getFinalidadeLabel(imovel.finalidade)}
        </span>
      </div>
      <div className="property-card-body">
        <p className="property-card-price">{preco}</p>
        <h3 className="property-card-title">{imovel.titulo}</h3>
        {imovel.endereco && (
          <p className="property-card-location">
            {imovel.endereco.bairro}, {imovel.endereco.cidade} - {imovel.endereco.estado}
          </p>
        )}
        <div className="property-card-features">
          {imovel.dormitorios > 0 && (
            <span>{imovel.dormitorios} {imovel.dormitorios === 1 ? 'quarto' : 'quartos'}</span>
          )}
          {imovel.banheiros > 0 && (
            <span>{imovel.banheiros} {imovel.banheiros === 1 ? 'banheiro' : 'banheiros'}</span>
          )}
          {imovel.vagasGaragem > 0 && (
            <span>{imovel.vagasGaragem} {imovel.vagasGaragem === 1 ? 'vaga' : 'vagas'}</span>
          )}
          <span>{imovel.areaTotal}m²</span>
        </div>
      </div>
    </Link>
  );
}
