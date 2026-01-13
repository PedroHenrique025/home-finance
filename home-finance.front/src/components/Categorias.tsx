/**
 * Componente de gerenciamento de Categorias
 * Permite listar e criar categorias de transações 
 */

import React, { useState, useEffect } from 'react';
import { Category, CreateCategoryDto, Purpose } from '../types';
import { listCategories, createCategory } from '../services/categoryService';
import '../styles/Categorias.css';

const Categorias: React.FC = () => {
  
  // Estado para armazenar a lista de categorias
  const [categorias, setCategorias] = useState<Category[]>([]);
  
  // Estado para controlar loading durante operações
  const [loading, setLoading] = useState<boolean>(false);
  
  // Estado para mensagens de erro
  const [erro, setErro] = useState<string>('');
  
  // Estado para mensagens de sucesso
  const [sucesso, setSucesso] = useState<string>('');
  
  // Estado do formulário de nova categoria
  const [novaCategoria, setNovaCategoria] = useState<CreateCategoryDto>({
    description: '',
    purpose: Purpose.Ambas
  });

  /**
   * useEffect para carregar a lista de categorias ao montar o componente
   */
  useEffect(() => {
    loadCategories();
  }, []);

  /**
   * Função para carregar todas as categorias do backend
   */
  const loadCategories = async () => {
    try {
      setLoading(true);
      setErro('');
      const data = await listCategories();
      setCategorias(data);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao carregar categorias');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função para lidar com mudanças no campo de descrição
   */
  const handleDescricaoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setNovaCategoria(prev => ({
      ...prev,
      description: e.target.value
    }));
  };

  /**
   * Função para lidar com mudanças no campo de finalidade
   */
  const handlePurposeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setNovaCategoria(prev => ({
      ...prev,
      purpose: parseInt(e.target.value)
    }));
  };

  /**
   * Função para submeter o formulário e criar uma nova categoria
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Limpa mensagens anteriores
    setErro('');
    setSucesso('');

    // Validação do formulário
    if (!novaCategoria.description.trim()) {
      setErro('Descrição é obrigatória');
      return;
    }

    try {
      setLoading(true);
      await createCategory(novaCategoria);
      setSucesso('Categoria criada com sucesso!');
      
      // Recarrega a lista de categorias
      await loadCategories();
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao criar categoria');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função auxiliar para obter a classe CSS do badge baseado na finalidade
   * @param purpose - Finalidade da categoria
   * @returns Nome da classe CSS
   */
  const getBadgeClass = (purpose: Purpose): string => {
    switch (purpose) {
      case Purpose.Despesa:
        return 'badge badge-danger';
      case Purpose.Receita:
        return 'badge badge-success';
      case Purpose.Ambas:
        return 'badge badge-info';
      default:
        return 'badge';
    }
  };

  /**
   * Limpa todos os campos do formulário
   */
  const handleLimparFormulario = () => {
    setNovaCategoria({ description: '', purpose: Purpose.Ambas });
    setErro('');
    setSucesso('');
  };

  /**
   * Função para contar quantas categorias existem por finalidade
   */
  const countByPurpose = () => {
    const despesas = categorias.filter(c => c.purpose === Purpose.Despesa).length;
    const receitas = categorias.filter(c => c.purpose === Purpose.Receita).length;
    const ambas = categorias.filter(c => c.purpose === Purpose.Ambas).length;
    
    return { despesas, receitas, ambas };
  };

  const stats = countByPurpose();

  return (
    <div className="categorias-container">
      <h1>Gerenciamento de Categorias</h1>

      {/* Mensagens de erro e sucesso */}
      {erro && <div className="mensagem erro">{erro}</div>}
      {sucesso && <div className="mensagem sucesso">{sucesso}</div>}

      {/* Estatísticas rápidas */}
      {categorias.length > 0 && (
        <div className="stats-section">
          <div className="stat-card">
            <span className="stat-label">Total de Categorias:</span>
            <span className="stat-value">{categorias.length}</span>
          </div>
          <div className="stat-card despesa">
            <span className="stat-label">Despesas:</span>
            <span className="stat-value">{stats.despesas}</span>
          </div>
          <div className="stat-card receita">
            <span className="stat-label">Receitas:</span>
            <span className="stat-value">{stats.receitas}</span>
          </div>
          <div className="stat-card ambas">
            <span className="stat-label">Ambas:</span>
            <span className="stat-value">{stats.ambas}</span>
          </div>
        </div>
      )}

      {/* Formulário para criar nova categoria */}
      <div className="formulario-section">
        <h2>Cadastrar Nova Categoria</h2>
        <form onSubmit={handleSubmit} className="formulario">
          <div className="form-group">
            <label htmlFor="descricao">Descrição:</label>
            <input
              type="text"
              id="descricao"
              name="description"
              value={novaCategoria.description}
              onChange={handleDescricaoChange}
              placeholder="Ex: Alimentação, Salário, Transporte..."
              disabled={loading}
              required
            />
            <small className="form-hint">
              Digite um nome descritivo para a categoria
            </small>
          </div>

          <div className="form-group">
            <label htmlFor="finalidade">Finalidade:</label>
            <select
              id="finalidade"
              name="purpose"
              value={novaCategoria.purpose}
              onChange={handlePurposeChange}
              disabled={loading}
              required
            >
              <option value={Purpose.Despesa}>Despesa</option>
              <option value={Purpose.Receita}>Receita</option>
              <option value={Purpose.Ambas}>Ambas</option>
            </select>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Salvando...' : 'Cadastrar Categoria'}
            </button>
            <button 
              type="button" 
              className="btn btn-secondary" 
              onClick={handleLimparFormulario}
              disabled={loading}
            >
              Limpar Campos
            </button>
          </div>
        </form>
      </div>

      {/* Lista de categorias cadastradas */}
      <div className="lista-section">
        <h2>Categorias Cadastradas</h2>
        {loading && <p>Carregando...</p>}
        
        {!loading && categorias.length === 0 && (
          <p className="mensagem-vazia">Nenhuma categoria cadastrada ainda.</p>
        )}

        {!loading && categorias.length > 0 && (
          <table className="tabela">
            <thead>
              <tr>
                <th>ID</th>
                <th>Descrição</th>
                <th>Finalidade</th>
              </tr>
            </thead>
            <tbody>
              {categorias.map((categoria) => (
                <tr key={categoria.id}>
                  <td>{categoria.id}</td>
                  <td>{categoria.description}</td>
                  <td>
                    <span className={getBadgeClass(categoria.purpose)}>
                      {categoria.purposeDescription}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default Categorias;
