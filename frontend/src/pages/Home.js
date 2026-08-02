import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import PropertyCard from '../components/PropertyCard';
import { imoveisApi } from '../services/api';
import './Home.css';

export default function Home() {
  const [destaques, setDestaques] = useState([]);
  const [loading, setLoading] = useState(true);
  const [busca, setBusca] = useState({ cidade: '', tipo: '', finalidade: '' });
  const navigate = useNavigate();

  useEffect(() => {
    imoveisApi.listar()
      .then(({ data }) => setDestaques(data.filter(i => i.status === 'Disponivel').slice(0, 6)))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleBusca = (e) => {
    e.preventDefault();
    const params = new URLSearchParams();
    if (busca.cidade) params.set('cidade', busca.cidade);
    if (busca.tipo) params.set('tipo', busca.tipo);
    if (busca.finalidade) params.set('finalidade', busca.finalidade);
    navigate(`/imoveis?${params.toString()}`);
  };

  return (
    <div className="home">
      <section className="hero">
        <div className="container hero-content">
          <h1 className="hero-title">Encontre o imóvel <span className="hero-highlight">ideal</span> para você</h1>
          <p className="hero-subtitle">Casas, apartamentos, terrenos e muito mais. A Pagnan Hub Imóveis conecta você ao seu próximo lar.</p>
          <form className="hero-search" onSubmit={handleBusca}>
            <input type="text" className="form-input" placeholder="Cidade..." value={busca.cidade} onChange={e=>setBusca({...busca,cidade:e.target.value})} />
            <select className="form-select" value={busca.tipo} onChange={e=>setBusca({...busca,tipo:e.target.value})}>
              <option value="">Tipo</option><option value="1">Casa</option><option value="2">Apartamento</option>
              <option value="3">Cobertura</option><option value="5">Kitnet</option><option value="6">Sobrado</option><option value="7">Terreno</option><option value="11">Sala Comercial</option>
            </select>
            <select className="form-select" value={busca.finalidade} onChange={e=>setBusca({...busca,finalidade:e.target.value})}>
              <option value="">Finalidade</option><option value="1">Comprar</option><option value="2">Alugar</option>
            </select>
            <button type="submit" className="btn btn-accent">Buscar</button>
          </form>
        </div>
      </section>

      <section className="section"><div className="container">
        <h2 className="section-title">Imóveis em destaque</h2>
        {loading ? <div className="loading">Carregando imóveis...</div> : destaques.length > 0 ? (
          <div className="property-grid">{destaques.map(im => <PropertyCard key={im.id} imovel={im} />)}</div>
        ) : <p className="empty-state">Nenhum imóvel disponível no momento.</p>}
      </div></section>

      <section className="stats-section"><div className="container stats-grid">
        <div className="stat-item"><span className="stat-number">{destaques.length}+</span><span className="stat-label">Imóveis disponíveis</span></div>
        <div className="stat-item"><span className="stat-number">2</span><span className="stat-label">Corretores especializados</span></div>
        <div className="stat-item"><span className="stat-number">Maringá</span><span className="stat-label">Região de atuação</span></div>
      </div></section>
    </div>
  );
}
