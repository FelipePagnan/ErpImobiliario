import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
});

// Interceptor para incluir token JWT
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor para tratar 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('usuario');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// === Imóveis ===
export const imoveisApi = {
  listar: () => api.get('/imoveis'),
  obterPorId: (id) => api.get(`/imoveis/${id}`),
  filtrar: (params) => api.get('/imoveis/filtrar', { params }),
  criar: (data) => api.post('/imoveis', data),
  atualizar: (id, data) => api.put(`/imoveis/${id}`, data),
  remover: (id) => api.delete(`/imoveis/${id}`),
  dashboard: () => api.get('/imoveis/dashboard'),
};

// === Auth ===
export const authApi = {
  login: (data) => api.post('/auth/login', data),
  registrar: (data) => api.post('/auth/registrar', data),
};

export default api;
