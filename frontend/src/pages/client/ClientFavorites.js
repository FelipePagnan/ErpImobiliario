import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import PropertyCard from '../../components/PropertyCard';
import { favoritosApi, clientesApi } from '../../services/api';

export default function ClientFavorites() {
  const [favoritos, setFavoritos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [clienteId, setClienteId] = useState(null);

  useEffect(() => {
    const load = async () => {
      try {
        const { data: cliente } = await clientesApi.meuPerfil();
        setClienteId(cliente.id);
        const { data } = await favoritosApi.listar(cliente.id);
        setFavoritos(data);
      } catch (err) { console.error(err); }
      finally { setLoading(false); }
    };
    load();
  }, []);

  const handleRemover = async (imovelId) => {
    try {
      await favoritosApi.remover(clienteId, imovelId);
      setFavoritos(prev => prev.filter(f => f.id !== imovelId));
    } catch { alert('Erro ao remover favorito'); }
  };

  return (
    <div style={{padding:'2rem 0'}}><div className="container">
      <h1 className="page-title">Meus Favoritos</h1>

      {loading ? <div className="loading">Carregando...</div> :
       favoritos.length === 0 ? (
        <div className="empty-state">
          <p>Você ainda não favoritou nenhum imóvel.</p>
          <Link to="/imoveis" className="btn btn-primary" style={{marginTop:'1rem'}}>Explorar imóveis</Link>
        </div>
      ) : (
        <>
          <p style={{color:'var(--color-text-muted)',fontSize:'0.875rem',marginBottom:'1.5rem'}}>{favoritos.length} imóvel(is) favoritado(s)</p>
          <div className="property-grid">
            {favoritos.map(imovel => (
              <div key={imovel.id} style={{position:'relative'}}>
                <PropertyCard imovel={imovel} />
                <button
                  onClick={() => handleRemover(imovel.id)}
                  style={{position:'absolute',top:12,right:12,background:'rgba(255,255,255,0.9)',border:'none',borderRadius:'50%',width:32,height:32,cursor:'pointer',fontSize:'1rem',display:'flex',alignItems:'center',justifyContent:'center',boxShadow:'0 2px 4px rgba(0,0,0,0.1)'}}
                  title="Remover dos favoritos"
                >❤️</button>
              </div>
            ))}
          </div>
        </>
      )}
    </div></div>
  );
}
