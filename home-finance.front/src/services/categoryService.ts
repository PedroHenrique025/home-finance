/**
 * Serviço de API para gerenciamento de Categorias
 * Contém todas as operações relacionadas a categorias de transações
 */

import axios from 'axios';
import { Category, CreateCategoryDto } from '../types';
import { API_ENDPOINTS } from '../config/api';

/**
 * Busca todas as categorias cadastradas no sistema
 * @returns Promise com array de categorias
 */
export const listCategories = async (): Promise<Category[]> => {
  try {
    const response = await axios.get<Category[]>(API_ENDPOINTS.CATEGORIAS.BASE);
    return response.data;
  } catch (error) {
    console.error('Erro ao listar categorias:', error);
    throw new Error('Não foi possível carregar a lista de categorias');
  }
};

/**
 * Busca uma categoria específica pelo ID
 * @param id - ID da categoria
 * @returns Promise com os dados da categoria
 */
export const getCategoryById = async (id: number): Promise<Category> => {
  try {
    const response = await axios.get<Category>(API_ENDPOINTS.CATEGORIAS.BY_ID(id));
    return response.data;
  } catch (error) {
    console.error(`Erro ao buscar categoria ${id}:`, error);
    throw new Error('Não foi possível carregar os dados da categoria');
  }
};

/**
 * Cria uma nova categoria no sistema
 * @param categoryDto - Dados da categoria a ser criada
 * @returns Promise com os dados da categoria criada (incluindo ID gerado)
 */
export const createCategory = async (categoryDto: CreateCategoryDto): Promise<Category> => {
  try {
    // Validação básica antes de enviar
    if (!categoryDto.description || categoryDto.description.trim() === '') {
      throw new Error('Descrição é obrigatória');
    }
    if (categoryDto.purpose === undefined || categoryDto.purpose === null) {
      throw new Error('Finalidade é obrigatória');
    }

    const response = await axios.post<Category>(API_ENDPOINTS.CATEGORIAS.BASE, categoryDto);
    return response.data;
  } catch (error) {
    console.error('Erro ao criar categoria:', error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível criar a categoria');
    }
    throw error;
  }
};
