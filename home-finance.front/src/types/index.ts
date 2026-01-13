/**
 * Tipos TypeScript para as entidades do sistema
 * Define as interfaces e enums que representam os dados do sistema de controle de gastos
 */

/**
 * Enumeração para os tipos de transação
 * - Despesa (0): saída de dinheiro
 * - Receita (1): entrada de dinheiro
 */
export enum TransactionType {
  Despesa = 0,
  Receita = 1
}

/**
 * Enumeração para as finalidades de uma categoria
 * - Despesa (0): categoria usada apenas para despesas
 * - Receita (1): categoria usada apenas para receitas
 * - Ambas (2): categoria que pode ser usada tanto para despesas quanto receitas
 */
export enum Purpose {
  Despesa = 0,
  Receita = 1,
  Ambas = 2
}

/**
 * Interface que representa uma pessoa no sistema
 */
export interface Person {
  /** Identificador único gerado automaticamente pelo sistema */
  id: number;
  /** Nome completo da pessoa */
  name: string;
  /** Idade da pessoa (número inteiro positivo) */
  age: number;
}

/**
 * Interface para criação de uma nova pessoa
 */
export interface CreatePersonDto {
  name: string;
  age: number;
}

/**
 * Interface para atualização de uma pessoa
 * Todos os campos são opcionais
 */
export interface UpdatePersonDto {
  name?: string;
  age?: number;
}

/**
 * Interface que representa uma categoria de transação
 */
export interface Category {
  id: number;
  description: string;
  purpose: Purpose;
  purposeDescription: string;
}

/**
 * Interface para criação de uma nova categoria
 */
export interface CreateCategoryDto {
  description: string;
  purpose: number;
}

/**
 * Interface que representa uma transação financeira
 */
export interface Transaction {
  id: number;
  description: string;
  /** Valor da transação*/
  amount: number;
  /** Tipo da transação (despesa ou receita) */
  type: TransactionType;
  categoryId: number;
  personId: number;
  categoryDescription?: string;
  personName?: string;

  createdAt: Date;
}

/**
 * Interface para criação de uma nova transação
 */
export interface CreateTransactionDto {
  description: string;
  amount: number;
  date: string;
  type: TransactionType;
  categoryId: number;
  personId: number;
}

/**
 * Interface para atualização de uma transação
 * Todos os campos são opcionais
 */
export interface UpdateTransactionDto {
  description?: string;
  amount?: number;
  date?: string;
  type?: TransactionType;
  categoryId?: number;
  personId?: number;
}

/**
 * Interface para os totais de uma pessoa
 * Utilizada na consulta de totais por pessoa
 */
export interface PersonTotalsDto {
  personId: number;
  personName: string;
  age: number;
  /** Soma total das receitas da pessoa */
  totalIncome: number;
  /** Soma total das despesas da pessoa */
  totalExpense: number;
  /** Saldo líquido (receitas - despesas) */
  balance: number;
}

/**
 * Interface para os totais de uma categoria
 * Utilizada na consulta de totais por categoria
 */
export interface CategoryTotalsDto {
  categoryId: number;
  categoryName: string;
  purpose: string;
  /** Soma total das receitas nesta categoria */
  totalIncome: number;
  /** Soma total das despesas nesta categoria */
  totalExpense: number;
  /** Saldo líquido (receitas - despesas) */
  balance: number;
}

/**
 * Interface para resposta da consulta de totais por pessoa
 * Inclui a lista de totais de cada pessoa e o total geral
 */
export interface PersonTotalsReportDto {
  /** Lista com os totais de cada pessoa */
  people: PersonTotalsDto[];
  /** Total geral de receitas (soma de todas as pessoas) */
  grandTotalIncome: number;
  /** Total geral de despesas (soma de todas as pessoas) */
  grandTotalExpense: number;
  /** Saldo geral (total receitas - total despesas) */
  grandBalance: number;
}

/**
 * Interface para resposta da consulta de totais por categoria
 * Inclui a lista de totais de cada categoria e o total geral
 */
export interface CategoryTotalsReportDto {
  /** Lista com os totais de cada categoria */
  categories: CategoryTotalsDto[];
  /** Total geral de receitas (soma de todas as categorias) */
  grandTotalIncome: number;
  /** Total geral de despesas (soma de todas as categorias) */
  grandTotalExpense: number;
  /** Saldo geral (total receitas - total despesas) */
  grandBalance: number;
}

/**
 * Funções auxiliares para conversão de enums
 */

/**
 * Retorna o nome legível de um tipo de transação
 */
export function getTransactionTypeName(type: TransactionType): string {
  return type === TransactionType.Receita ? 'Receita' : 'Despesa';
}

/**
 * Retorna o nome legível de uma finalidade
 */
export function getPurposeName(purpose: Purpose): string {
  if (typeof purpose === 'string') {
    return purpose; // Já é string
  }
  return Purpose[purpose] || 'Desconhecido';
}
