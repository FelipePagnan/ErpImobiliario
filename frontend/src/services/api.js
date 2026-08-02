import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

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

export const imoveisApi = {
  listar: () => api.get('/imoveis'),
  obterPorId: (id) => api.get(`/imoveis/${id}`),
  filtrar: (params) => api.get('/imoveis/filtrar', { params }),
  criar: (data) => api.post('/imoveis', data),
  atualizar: (id, data) => api.put(`/imoveis/${id}`, data),
  remover: (id) => api.delete(`/imoveis/${id}`),
  dashboard: () => api.get('/imoveis/dashboard'),
};

export const authApi = {
  login: (data) => api.post('/auth/login', data),
  registrar: (data) => api.post('/auth/registrar', data),
};

export const clientesApi = {
  meuPerfil: () => api.get('/clientes/me'),
};

export const favoritosApi = {
  listar: (clienteId) => api.get(`/favoritos/${clienteId}`),
  adicionar: (clienteId, imovelId) => api.post(`/favoritos/${clienteId}/${imovelId}`),
  remover: (clienteId, imovelId) => api.delete(`/favoritos/${clienteId}/${imovelId}`),
  verificar: (clienteId, imovelId) => api.get(`/favoritos/${clienteId}/${imovelId}/check`),
};

export const visitasApi = {
  listar: () => api.get('/visitas'),
  obterPorId: (id) => api.get(`/visitas/${id}`),
  porCliente: (clienteId) => api.get(`/visitas/cliente/${clienteId}`),
  porCorretor: (corretorId) => api.get(`/visitas/corretor/${corretorId}`),
  criar: (data) => api.post('/visitas', data),
  atualizar: (id, data) => api.put(`/visitas/${id}`, data),
  cancelar: (id) => api.post(`/visitas/${id}/cancelar`),
};

export const contratosApi = {
  listar: () => api.get('/contratos'),
  obterPorId: (id) => api.get(`/contratos/${id}`),
  vencendo: (dias = 30) => api.get(`/contratos/vencendo?dias=${dias}`),
  criar: (data) => api.post('/contratos', data),
  atualizar: (id, data) => api.put(`/contratos/${id}`, data),
  rescindir: (id, motivo) => api.post(`/contratos/${id}/rescindir`, { motivo }),
  renovar: (id, novaDataFim, novoValor) => api.post(`/contratos/${id}/renovar`, { novaDataFim, novoValor }),
};

export const financeiroApi = {
  resumo: () => api.get('/financeiro/resumo'),
  lancamentos: () => api.get('/financeiro/lancamentos'),
  porPeriodo: (inicio, fim) => api.get(`/financeiro/lancamentos/periodo?inicio=${inicio}&fim=${fim}`),
  criarLancamento: (data) => api.post('/financeiro/lancamentos', data),
  atualizarLancamento: (id, data) => api.put(`/financeiro/lancamentos/${id}`, data),
  pagarLancamento: (id) => api.post(`/financeiro/lancamentos/${id}/pagar`),
  comissoes: () => api.get('/financeiro/comissoes'),
  criarComissao: (data) => api.post('/financeiro/comissoes', data),
  pagarComissao: (id) => api.post(`/financeiro/comissoes/${id}/pagar`),
};

export const crmApi = {
  listar: () => api.get('/crm'),
  obterPorId: (id) => api.get(`/crm/${id}`),
  criar: (data) => api.post('/crm', data),
  atualizar: (id, data) => api.put(`/crm/${id}`, data),
  remover: (id) => api.delete(`/crm/${id}`),
  registrarContato: (id, data) => api.post(`/crm/${id}/contato`, data),
  imoveisCompativeis: (id) => api.get(`/crm/${id}/imoveis-compativeis`),
};

export const auditoriaApi = {
  listar: (limite = 200) => api.get(`/auditoria?limite=${limite}`),
  porEntidade: (entidade, id) => api.get(`/auditoria/${entidade}/${id}`),
};

export default api;
