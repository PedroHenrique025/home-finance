/**
 * Serviço de API para Consultas e Relatórios
 * Contém operações para gerar relatórios de totais por pessoa e categoria
 */

import axios from 'axios';
import { PersonTotalsReportDto, CategoryTotalsReportDto } from '../types';
import { API_ENDPOINTS } from '../config/api';

/**
 * Busca os totais por pessoa
 * Retorna uma lista com cada pessoa e seus totais de receitas, despesas e saldo,
 * além do total geral de todas as pessoas
 * 
 * @returns Promise com os dados da consulta de totais por pessoa
 */
export const queryTotalsByPerson = async (): Promise<PersonTotalsReportDto> => {
  try {
    const response = await axios.get<PersonTotalsReportDto>(
      API_ENDPOINTS.CONSULTAS.TOTAIS_POR_PESSOA
    );
    return response.data;
  } catch (error) {
    console.error('Erro ao consultar totais por pessoa:', error);
    throw new Error('Não foi possível carregar os totais por pessoa');
  }
};

/**
 * Busca os totais por categoria (funcionalidade opcional)
 * Retorna uma lista com cada categoria e seus totais de receitas, despesas e saldo,
 * além do total geral de todas as categorias
 * 
 * @returns Promise com os dados da consulta de totais por categoria
 */
export const queryTotalsByCategory = async (): Promise<CategoryTotalsReportDto> => {
  try {
    const response = await axios.get<CategoryTotalsReportDto>(
      API_ENDPOINTS.CONSULTAS.TOTAIS_POR_CATEGORIA
    );
    return response.data;
  } catch (error) {
    console.error('Erro ao consultar totais por categoria:', error);
    throw new Error('Não foi possível carregar os totais por categoria');
  }
};
