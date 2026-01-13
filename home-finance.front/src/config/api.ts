/**
 * Configuração base da API
 * Centraliza a URL base da API e outras configurações relacionadas
 */

/**
 * URL base da API .NET
 * Por padrão, o backend .NET roda na porta 5000 ou 5001 (HTTPS)
 * Ajuste conforme a configuração do seu backend
 */
export const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5203';

/**
 * Endpoints da API organizados por recurso
 * Facilita a manutenção e mudanças de rotas
 */
export const API_ENDPOINTS = {
  /** Endpoints relacionados a pessoas */
  PESSOAS: {
    BASE: `${API_BASE_URL}/api/persons`,
    BY_ID: (id: number) => `${API_BASE_URL}/api/persons/${id}`,
  },
  /** Endpoints relacionados a categorias */
  CATEGORIAS: {
    BASE: `${API_BASE_URL}/api/categories`,
    BY_ID: (id: number) => `${API_BASE_URL}/api/categories/${id}`,
  },
  /** Endpoints relacionados a transações */
  TRANSACOES: {
    BASE: `${API_BASE_URL}/api/transactions`,
    BY_ID: (id: number) => `${API_BASE_URL}/api/transactions/${id}`,
  },
  /** Endpoints relacionados a consultas */
  CONSULTAS: {
    TOTAIS_POR_PESSOA: `${API_BASE_URL}/api/Reports/persons`,
    TOTAIS_POR_CATEGORIA: `${API_BASE_URL}/api/Reports/categories`,
  },
};
