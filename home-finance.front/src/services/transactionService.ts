/**
 * Serviço de API para gerenciamento de Transações
 * Contém todas as operações relacionadas a transações financeiras
 */

import axios from 'axios';
import { Transaction, CreateTransactionDto, UpdateTransactionDto, TransactionType } from '../types';
import { API_ENDPOINTS } from '../config/api';

/**
 * Busca todas as transações cadastradas no sistema
 * @returns Promise com array de transações
 */
export const listTransactions = async (): Promise<Transaction[]> => {
  try {
    const response = await axios.get<Transaction[]>(API_ENDPOINTS.TRANSACOES.BASE);
    return response.data;
  } catch (error) {
    console.error('Erro ao listar transações:', error);
    throw new Error('Não foi possível carregar a lista de transações');
  }
};

/**
 * Busca uma transação específica pelo ID
 * @param id - ID da transação
 * @returns Promise com os dados da transação
 */
export const getTransactionById = async (id: number): Promise<Transaction> => {
  try {
    const response = await axios.get<Transaction>(API_ENDPOINTS.TRANSACOES.BY_ID(id));
    return response.data;
  } catch (error) {
    console.error(`Erro ao buscar transação ${id}:`, error);
    throw new Error('Não foi possível carregar os dados da transação');
  }
};

/**
 * Cria uma nova transação no sistema
 * @param transactionDto - Dados da transação a ser criada
 * @returns Promise com os dados da transação criada (incluindo ID gerado)
 */
export const createTransaction = async (transactionDto: CreateTransactionDto): Promise<Transaction> => {
  try {
    // Validações básicas antes de enviar
    if (!transactionDto.description || transactionDto.description.trim() === '') {
      throw new Error('Descrição é obrigatória');
    }
    if (!transactionDto.amount || transactionDto.amount <= 0) {
      throw new Error('Valor deve ser um número positivo');
    }
    if (!transactionDto.date) {
      throw new Error('Data é obrigatória');
    }
    if (transactionDto.type === undefined || transactionDto.type === null) {
      throw new Error('Tipo de transação é obrigatório');
    }
    if (!transactionDto.categoryId) {
      throw new Error('Categoria é obrigatória');
    }
    if (!transactionDto.personId) {
      throw new Error('Pessoa é obrigatória');
    }

    const response = await axios.post<Transaction>(API_ENDPOINTS.TRANSACOES.BASE, transactionDto);
    return response.data;
  } catch (error) {
    console.error('Erro ao criar transação:', error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível criar a transação');
    }
    throw error;
  }
};

/**
 * Atualiza uma transação existente no sistema
 * @param id - ID da transação a ser atualizada
 * @param transactionDto - Dados a serem atualizados (campos opcionais)
 * @returns Promise com os dados da transação atualizada
 */
export const updateTransaction = async (id: number, transactionDto: UpdateTransactionDto): Promise<Transaction> => {
  try {
    const response = await axios.put<Transaction>(API_ENDPOINTS.TRANSACOES.BY_ID(id), transactionDto);
    return response.data;
  } catch (error) {
    console.error(`Erro ao atualizar transação ${id}:`, error);
    if (axios.isAxiosError(error) && error.response) {
      throw new Error(error.response.data.message || 'Não foi possível atualizar a transação');
    }
    throw new Error('Não foi possível atualizar a transação');
  }
};

/**
 * Filtra transações por pessoa
 * Para exibir apenas as transações de uma pessoa específica
 * @param transactions - Array de todas as transações
 * @param personId - ID da pessoa para filtrar
 * @returns Array de transações da pessoa
 */
export const filterTransactionsByPerson = (transactions: Transaction[], personId: number): Transaction[] => {
  return transactions.filter(t => t.personId === personId);
};

/**
 * Filtra transações por categoria
 * Para exibir apenas as transações de uma categoria específica
 * @param transactions - Array de todas as transações
 * @param categoryId - ID da categoria para filtrar
 * @returns Array de transações da categoria
 */
export const filterTransactionsByCategory = (transactions: Transaction[], categoryId: number): Transaction[] => {
  return transactions.filter(t => t.categoryId === categoryId);
};

/**
 * Filtra transações por tipo (Despesa ou Receita)
 * @param transactions - Array de todas as transações
 * @param type - Tipo de transação para filtrar
 * @returns Array de transações do tipo especificado
 */
export const filterTransactionsByType = (transactions: Transaction[], type: TransactionType): Transaction[] => {
  return transactions.filter(t => t.type === type);
};
