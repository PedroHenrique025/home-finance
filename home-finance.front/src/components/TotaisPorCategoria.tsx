/**
 * Componente de Consulta de Totais por Categoria (Funcionalidade Opcional)
 * Exibe um relatório com os totais de receitas, despesas e saldo de cada categoria
 */

import React, { useState, useEffect } from 'react';
import { CategoryTotalsReportDto } from '../types';
import { queryTotalsByCategory } from '../services/queryService';
import '../styles/Consultas.css';

const TotaisPorCategoria: React.FC = () => {
  // Estado para armazenar os dados da consulta
  const [dados, setDados] = useState<CategoryTotalsReportDto | null>(null);
  
  // Estados de controle
  const [loading, setLoading] = useState<boolean>(false);
  const [erro, setErro] = useState<string>('');

  /**
   * Carrega os dados ao montar o componente
   */
  useEffect(() => {
    loadData();
  }, []);

  /**
   * Função para carregar os totais por categoria do backend
   */
  const loadData = async () => {
    try {
      setLoading(true);
      setErro('');
      const resultado = await queryTotalsByCategory();
      setDados(resultado);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao carregar totais por categoria');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Formata valor monetário para exibição em Real Brasileiro
   */
  const formatCurrency = (value: number | undefined): string => {
    if (value === undefined || value === null || isNaN(value)) {
      return 'R$ 0,00';
    }
    return value.toLocaleString('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    });
  };

  /**
   * Retorna a classe CSS apropriada baseada no saldo
   */
  const getBalanceClass = (balance: number): string => {
    if (balance > 0) return 'saldo-positivo';
    if (balance < 0) return 'saldo-negativo';
    return 'saldo-neutro';
  };

  return (
    <div className="consulta-container">
      <h1>Totais por Categoria</h1>
      <p className="descricao">
        Visualize o total de receitas, despesas e saldo de cada categoria cadastrada no sistema.
      </p>

      {/* Mensagem de erro */}
      {erro && <div className="mensagem erro">{erro}</div>}

      {/* Loading */}
      {loading && <div className="loading">Carregando dados...</div>}

      {/* Conteúdo */}
      {!loading && dados && (
        <>
          {/* Tabela de totais por categoria */}
          {dados.categories.length === 0 ? (
            <div className="mensagem-vazia">
              Nenhuma categoria cadastrada ou sem transações registradas.
            </div>
          ) : (
            <div className="tabela-wrapper">
              <table className="tabela tabela-consulta">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Descrição</th>
                    <th>Finalidade</th>
                    <th>Total Receitas</th>
                    <th>Total Despesas</th>
                    <th>Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  {dados.categories.map((item) => (
                    <tr key={item.categoryId}>
                      <td>{item.categoryId}</td>
                      <td className="descricao-categoria">
                        {item.categoryName}
                      </td>
                      <td>
                        <span className="badge">
                          {item.purpose}
                        </span>
                      </td>
                      <td className="valor-positivo">{formatCurrency(item.totalIncome)}</td>
                      <td className="valor-negativo">{formatCurrency(item.totalExpense)}</td>
                      <td className={getBalanceClass(item.balance)}>
                        <strong>{formatCurrency(item.balance)}</strong>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {/* Total Geral */}
          <div className="total-geral-section">
            <h2>Total Geral</h2>
            <div className="total-geral-cards">
              <div className="total-card receita">
                <div className="total-card-label">Total de Receitas</div>
                <div className="total-card-valor">
                  {formatCurrency(dados.grandTotalIncome)}
                </div>
              </div>
              
              <div className="total-card despesa">
                <div className="total-card-label">Total de Despesas</div>
                <div className="total-card-valor">
                  {formatCurrency(dados.grandTotalExpense)}
                </div>
              </div>
              
              <div className={`total-card ${getBalanceClass(dados.grandBalance)}`}>
                <div className="total-card-label">Saldo Líquido</div>
                <div className="total-card-valor">
                  {formatCurrency(dados.grandBalance)}
                </div>
                <div className="total-card-info">
                  {dados.grandBalance >= 0 
                    ? 'Saldo positivo' 
                    : 'Saldo negativo'}
                </div>
              </div>
            </div>         
          </div>

          {/* Botão para atualizar */}
          <div className="acoes-section">
            <button 
              className="btn btn-success" 
              onClick={loadData}
              disabled={loading}
            >
              Atualizar Dados
            </button>
          </div>
        </>
      )}
    </div>
  );
};

export default TotaisPorCategoria;
