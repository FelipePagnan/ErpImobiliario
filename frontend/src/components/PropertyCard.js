import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { favoritosApi, clientesApi } from '../services/api';
import { useToast } from './Toast';
import './PropertyCard.css';

const placeholderImg = 'data:image/svg+xml,' + encodeURIComponent(
  '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 400 260" fill="none"><rect width="400" height="260" fill="#e2e5ea"/><text x="200" y="130" text-anchor="middle" dy=".3em" fill="#8b95a5" font-family="sans-serif" font-size="16">Sem foto</text></svg>'
);

function formatPrice(value) {
  if (!value) return null;
  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
}

function getFinalidadeLabel(f) {
  const map = { 'Venda':'Venda', 'Locacao':'Locação', 'VendaELocacao':'Venda / Locação', 'Troca':'Troca' };
  return map[f] || f;
}

function getFinalidadeBadgeClass(f) {
  const map = { 'Venda':'badge-venda', 'Locacao':'badge-locacao', 'VendaELocacao':'badge-vendaElocacao', 'Troca':'badge-venda' };
  return map[f] || 'badge-venda';
}

export default function PropertyCard({ imovel, onFavChange }) {
  const { usuario } = useAuth();
  const toast = useToast();
  const [fav, setFav] = useState(false);
  const [clienteId, setClienteId] = useState(null);
  const isCliente = usuario?.perfil === 'Cliente';

  useEffect(() => {
    if (!isCliente) return;
    const check = async () => {
      try {
        const { data: c } = await clientesApi.meuPerfil();
        setClienteId(c.id);
        const { data } = await favoritosApi.verificar(c.id, imovel.id);
        setFav(data.favorito);
      } catch {}
    };
    check();
  }, [isCliente, imovel.id]);

  const toggleFav = async (e) => {
    e.preventDefault();
    e.stopPropagation();
    if (!isCliente || !clienteId) return;
    try {
      if (fav) { await favoritosApi.remover(clienteId, imovel.id); setFav(false); toast.info('Removido dos favoritos'); }
      else { await favoritosApi.adicionar(clienteId, imovel.id); setFav(true); toast.success('Adicionado aos favoritos!'); }
      if (onFavChange) onFavChange();
    } catch { toast.error('Erro ao atualizar favorito'); }
  };

  const preco = imovel.precoVenda ? formatPrice(imovel.precoVenda) : imovel.precoLocacao ? `${formatPrice(imovel.precoLocacao)}/mês` : 'Consulte';

  return (
    <Link to={`/imoveis/${imovel.id}`} className="property-card fade-in">
      <div className="property-card-img">
        <img src={imovel.fotoPrincipalUrl || placeholderImg} alt={imovel.titulo} />
        <span className={`badge ${getFinalidadeBadgeClass(imovel.finalidade)}`}>{getFinalidadeLabel(imovel.finalidade)}</span>
        {isCliente && (
          <button className={`fav-btn ${fav ? 'fav-active' : ''}`} onClick={toggleFav} title={fav ? 'Remover favorito' : 'Favoritar'}>
            {fav ? '❤️' : '🤍'}
          </button>
        )}
      </div>
      <div className="property-card-body">
        <p className="property-card-price">{preco}</p>
        <h3 className="property-card-title">{imovel.titulo}</h3>
        {imovel.endereco && <p className="property-card-location">{imovel.endereco.bairro}, {imovel.endereco.cidade} - {imovel.endereco.estado}</p>}
        <div className="property-card-features">
          {imovel.dormitorios > 0 && <span>{imovel.dormitorios} {imovel.dormitorios === 1 ? 'quarto' : 'quartos'}</span>}
          {imovel.banheiros > 0 && <span>{imovel.banheiros} {imovel.banheiros === 1 ? 'banheiro' : 'banheiros'}</span>}
          {imovel.vagasGaragem > 0 && <span>{imovel.vagasGaragem} {imovel.vagasGaragem === 1 ? 'vaga' : 'vagas'}</span>}
          <span>{imovel.areaTotal}m²</span>
        </div>
      </div>
    </Link>
  );
}
