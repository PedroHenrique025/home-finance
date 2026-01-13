/**
 * Serviço de API para gerenciamento de Pessoas
 * Contém todas as operações CRUD relacionadas a pessoas
 */

import axios from 'axios';
import { Person, CreatePersonDto, UpdatePersonDto } from '../types';
import { API_ENDPOINTS } from '../config/api';

/**
 * Busca todas as pessoas cadastradas no sistema
 * @returns Promise com array de pessoas
 */
export const listPeople = async (): Promise<Person[]> => {
  try {
    const response = await axios.get<Person[]>(API_ENDPOINTS.PESSOAS.BASE);
    return response.data;
  } catch (error) {
    console.error('Erro ao listar pessoas:', error);
    throw new Error('Não foi possível carregar a lista de pessoas');
  }
};

/**
 * Busca uma pessoa específica pelo ID
 * @param id - ID da pessoa
 * @returns Promise com os dados da pessoa
 */
export const getPersonById = async (id: number): Promise<Person> => {
  try {
    const response = await axios.get<Person>(API_ENDPOINTS.PESSOAS.BY_ID(id));
    return response.data;
  } catch (error) {
    console.error(`Erro ao buscar pessoa ${id}:`, error);
    throw new Error('Não foi possível carregar os dados da pessoa');
  }
};

/**
 * Cria uma nova pessoa no sistema
 * @param personDto - Dados da pessoa a ser criada
 * @returns Promise com os dados da pessoa criada (incluindo ID gerado)
 */
export const createPerson = async (personDto: CreatePersonDto): Promise<Person> => {
  try {
    // Validação básica antes de enviar
    if (!personDto.name || personDto.name.trim() === '') {
      throw new Error('Nome é obrigatório');
    }
    if (!personDto.age || personDto.age < 0) {
      throw new Error('Idade deve ser um número positivo');
    }

    const response = await axios.post<Person>(API_ENDPOINTS.PESSOAS.BASE, personDto);
    return response.data;
  } catch (error) {
    console.error('Erro ao criar pessoa:', error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível criar a pessoa');
    }
    throw error;
  }
};

/**
 * Deleta uma pessoa do sistema e suas transações
 * conforme especificado nos requisitos
 * @param id - ID da pessoa a ser deletada
 */
export const deletePerson = async (id: number): Promise<void> => {
  try {
    await axios.delete(API_ENDPOINTS.PESSOAS.BY_ID(id));
  } catch (error) {
    console.error(`Erro ao deletar pessoa ${id}:`, error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível deletar a pessoa');
    }
    throw new Error('Não foi possível deletar a pessoa');
  }
};

/**
 * Atualiza uma pessoa existente no sistema
 * @param id - ID da pessoa a ser atualizada
 * @param personDto - Dados a serem atualizados (campos opcionais)
 * @returns Promise com os dados da pessoa atualizada
 */
export const updatePerson = async (id: number, personDto: UpdatePersonDto): Promise<Person> => {
  try {
    const response = await axios.patch<Person>(API_ENDPOINTS.PESSOAS.BY_ID(id), personDto);
    return response.data;
  } catch (error) {
    console.error(`Erro ao atualizar pessoa ${id}:`, error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível atualizar a pessoa');
    }
    throw new Error('Não foi possível atualizar a pessoa');
  }
};
