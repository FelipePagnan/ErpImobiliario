import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import PropertyCard from '../components/PropertyCard';
import { imoveisApi } from '../services/api';
import './PropertyList.css';

export default function PropertyList() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [imoveis, setImoveis] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filtros, setFiltros] = useState({
    cidade: searchParams.get('cidade') || '',
    bairro: searchParams.get('bairro') || '',
    tipo: searchParams.get('tipo') || '',
    finalidade: searchParams.get('finalidade') || '',
    precoMin: searchParams.get('precoMin') || '',
    precoMax: searchParams.get('precoMax') || '',
    dormitoriosMin: searchParams.get('dormitoriosMin') || '',
  });

  const fetchImoveis = async (params) => {
    setLoading(true);
    try {
      const cleanParams = {};
      Object.entries(params).forEach(([key, val]) => {
        if (val) cleanParams[key] = val;
      });

      const { data } = Object.keys(cleanParams).length > 0
        ? await imoveisApi.filtrar(cleanParams)
        : await imoveisApi.listar();

      setImoveis(data.filter(i => i.status === 'Disponivel'));
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchImoveis(filtros);
  }, []); // eslint-disable-line

  const handleFiltrar = (e) => {
    e.preventDefault();
    const params = new URLSearchParams();
    Object.entries(filtros).forEach(([key, val]) => {
      if (val) params.set(key, val);
    });
    setSearchParams(params);
    fetchImoveis(filtros);
  };

  const handleLimpar = () => {
    setFiltros({ cidade: '', bairro: '', tipo: '', finalidade: '', precoMin: '', precoMax: '', dormitoriosMin: '' });
    setSearchParams({});
    fetchImoveis({});
  };

  return (
    <div className="property-list-page">
      <div className="container">
        <h1 className="page-title">Imóveis disponíveis</h1>

        <form className="filters-bar" onSubmit={handleFiltrar}>
          <input
            type="text"
            className="form-input"
            placeholder="Cidade"
            value={filtros.cidade}
            onChange={(e) => setFiltros({ ...filtros, cidade: e.target.value })}
          />
          <input
            type="text"
            className="form-input"
            placeholder="Bairro"
            value={filtros.bairro}
            onChange={(e) => setFiltros({ ...filtros, bairro: e.target.value })}
          />
          <select
            className="form-select"
            value={filtros.tipo}
            onChange={(e) => setFiltros({ ...filtros, tipo: e.target.value })}
          >
            <option value="">Tipo</option>
            <option value="1">Casa</option>
            <option value="2">Apartamento</option>
            <option value="3">Cobertura</option>
            <option value="4">Studio</option>
            <option value="5">Kitnet</option>
            <option value="6">Sobrado</option>
            <option value="7">Terreno</option>
            <option value="10">Galpão</option>
            <option value="11">Sala Comercial</option>
            <option value="12">Loja</option>
          </select>
          <select
            className="form-select"
            value={filtros.finalidade}
            onChange={(e) => setFiltros({ ...filtros, finalidade: e.target.value })}
          >
            <option value="">Finalidade</option>
            <option value="1">Comprar</option>
            <option value="2">Alugar</option>
          </select>
          <select
            className="form-select"
            value={filtros.dormitoriosMin}
            onChange={(e) => setFiltros({ ...filtros, dormitoriosMin: e.target.value })}
          >
            <option value="">Quartos</option>
            <option value="1">1+</option>
            <option value="2">2+</option>
            <option value="3">3+</option>
            <option value="4">4+</option>
          </select>
          <div className="filters-actions">
            <button type="submit" className="btn btn-primary">Filtrar</button>
            <button type="button" className="btn btn-outline" onClick={handleLimpar}>Limpar</button>
          </div>
        </form>

        <p className="results-count">
          {loading ? 'Buscando...' : `${imoveis.length} imóvel(is) encontrado(s)`}
        </p>

        {loading ? (
          <div className="loading">Carregando...</div>
        ) : imoveis.length > 0 ? (
          <div className="property-grid">
            {imoveis.map((imovel) => (
              <PropertyCard key={imovel.id} imovel={imovel} />
            ))}
          </div>
        ) : (
          <div className="empty-state">
            <p>Nenhum imóvel encontrado com os filtros selecionados.</p>
            <button className="btn btn-outline" onClick={handleLimpar} style={{ marginTop: '1rem' }}>
              Limpar filtros
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
