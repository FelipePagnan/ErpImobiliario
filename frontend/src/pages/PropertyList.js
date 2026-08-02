import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import PropertyCard from '../components/PropertyCard';
import { imoveisApi } from '../services/api';
import './PropertyList.css';

const ITEMS_PER_PAGE = 9;

export default function PropertyList() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [imoveis, setImoveis] = useState([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState('recente');
  const [filtros, setFiltros] = useState({
    cidade: searchParams.get('cidade') || '', bairro: searchParams.get('bairro') || '',
    tipo: searchParams.get('tipo') || '', finalidade: searchParams.get('finalidade') || '',
    precoMin: searchParams.get('precoMin') || '', precoMax: searchParams.get('precoMax') || '',
    dormitoriosMin: searchParams.get('dormitoriosMin') || '', areaMin: searchParams.get('areaMin') || '',
    vagasMin: searchParams.get('vagasMin') || '',
  });

  const fetchImoveis = async (params) => {
    setLoading(true);
    try {
      const clean = {}; Object.entries(params).forEach(([k,v]) => { if(v) clean[k] = v; });
      const { data } = Object.keys(clean).length > 0 ? await imoveisApi.filtrar(clean) : await imoveisApi.listar();
      setImoveis(data.filter(i => i.status === 'Disponivel'));
    } catch (err) { console.error(err); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchImoveis(filtros); }, []); // eslint-disable-line

  const handleFiltrar = (e) => {
    e.preventDefault();
    const params = new URLSearchParams();
    Object.entries(filtros).forEach(([k,v]) => { if(v) params.set(k,v); });
    setSearchParams(params); setPage(1); fetchImoveis(filtros);
  };

  const handleLimpar = () => {
    const empty = {cidade:'',bairro:'',tipo:'',finalidade:'',precoMin:'',precoMax:'',dormitoriosMin:'',areaMin:'',vagasMin:''};
    setFiltros(empty); setSearchParams({}); setPage(1); fetchImoveis({});
  };

  const sorted = [...imoveis].sort((a,b) => {
    if (sortBy === 'menor') return (a.precoVenda||a.precoLocacao||0) - (b.precoVenda||b.precoLocacao||0);
    if (sortBy === 'maior') return (b.precoVenda||b.precoLocacao||0) - (a.precoVenda||a.precoLocacao||0);
    if (sortBy === 'area') return b.areaTotal - a.areaTotal;
    return 0; // recente (default order from API)
  });

  const totalPages = Math.ceil(sorted.length / ITEMS_PER_PAGE);
  const paginados = sorted.slice((page-1)*ITEMS_PER_PAGE, page*ITEMS_PER_PAGE);

  return (
    <div className="property-list-page"><div className="container">
      <h1 className="page-title">Imóveis disponíveis</h1>

      <form className="filters-bar" onSubmit={handleFiltrar}>
        <input className="form-input" placeholder="Cidade" value={filtros.cidade} onChange={e=>setFiltros({...filtros,cidade:e.target.value})} />
        <input className="form-input" placeholder="Bairro" value={filtros.bairro} onChange={e=>setFiltros({...filtros,bairro:e.target.value})} />
        <select className="form-select" value={filtros.tipo} onChange={e=>setFiltros({...filtros,tipo:e.target.value})}>
          <option value="">Tipo</option><option value="1">Casa</option><option value="2">Apartamento</option>
          <option value="3">Cobertura</option><option value="5">Kitnet</option><option value="6">Sobrado</option>
          <option value="7">Terreno</option><option value="10">Galpão</option><option value="11">Sala Comercial</option>
        </select>
        <select className="form-select" value={filtros.finalidade} onChange={e=>setFiltros({...filtros,finalidade:e.target.value})}>
          <option value="">Finalidade</option><option value="1">Comprar</option><option value="2">Alugar</option>
        </select>
        <select className="form-select" value={filtros.dormitoriosMin} onChange={e=>setFiltros({...filtros,dormitoriosMin:e.target.value})}>
          <option value="">Quartos</option><option value="1">1+</option><option value="2">2+</option><option value="3">3+</option><option value="4">4+</option>
        </select>
        <input className="form-input" type="number" placeholder="Preço mín" value={filtros.precoMin} onChange={e=>setFiltros({...filtros,precoMin:e.target.value})} />
        <input className="form-input" type="number" placeholder="Preço máx" value={filtros.precoMax} onChange={e=>setFiltros({...filtros,precoMax:e.target.value})} />
        <input className="form-input" type="number" placeholder="Área mín (m²)" value={filtros.areaMin} onChange={e=>setFiltros({...filtros,areaMin:e.target.value})} />
        <select className="form-select" value={filtros.vagasMin} onChange={e=>setFiltros({...filtros,vagasMin:e.target.value})}>
          <option value="">Vagas</option><option value="1">1+</option><option value="2">2+</option><option value="3">3+</option>
        </select>
        <div className="filters-actions">
          <button type="submit" className="btn btn-primary">Filtrar</button>
          <button type="button" className="btn btn-outline" onClick={handleLimpar}>Limpar</button>
        </div>
      </form>

      <div className="results-bar">
        <p className="results-count">{loading ? 'Buscando...' : `${imoveis.length} imóvel(is) encontrado(s)`}</p>
        <select className="form-select sort-select" value={sortBy} onChange={e=>{setSortBy(e.target.value);setPage(1);}}>
          <option value="recente">Mais recentes</option><option value="menor">Menor preço</option>
          <option value="maior">Maior preço</option><option value="area">Maior área</option>
        </select>
      </div>

      {loading ? <div className="loading">Carregando...</div> : paginados.length > 0 ? (
        <>
          <div className="property-grid">{paginados.map(im => <PropertyCard key={im.id} imovel={im} />)}</div>
          {totalPages > 1 && (
            <div className="pagination">
              <button className="btn btn-outline btn-sm" disabled={page<=1} onClick={()=>setPage(p=>p-1)}>← Anterior</button>
              <span className="page-info">Página {page} de {totalPages}</span>
              <button className="btn btn-outline btn-sm" disabled={page>=totalPages} onClick={()=>setPage(p=>p+1)}>Próxima →</button>
            </div>
          )}
        </>
      ) : (
        <div className="empty-state"><p>Nenhum imóvel encontrado.</p><button className="btn btn-outline" onClick={handleLimpar} style={{marginTop:'1rem'}}>Limpar filtros</button></div>
      )}
    </div></div>
  );
}
