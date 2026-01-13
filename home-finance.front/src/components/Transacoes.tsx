/**
 * Componente de gerenciamento de Transações
 * Permite listar e criar transações financeiras (receitas e despesas)
 */

import React, { useState, useEffect } from 'react';
import { 
  Transaction, 
  CreateTransactionDto,
  UpdateTransactionDto, 
  TransactionType, 
  Person, 
  Category,
  Purpose,
  getTransactionTypeName
} from '../types';
import { listTransactions, createTransaction, updateTransaction } from '../services/transactionService';
import { listPeople } from '../services/personService';
import { listCategories } from '../services/categoryService';
import '../styles/Transacoes.css';

const Transacoes: React.FC = () => {
  // Estados para dados
  const [transacoes, setTransacoes] = useState<Transaction[]>([]);
  const [pessoas, setPessoas] = useState<Person[]>([]);
  const [categorias, setCategorias] = useState<Category[]>([]);
  
  // Estados de controle
  const [loading, setLoading] = useState<boolean>(false);
  const [erro, setErro] = useState<string>('');
  const [sucesso, setSucesso] = useState<string>('');
  
  // Estado do formulário
  const [novaTransacao, setNovaTransacao] = useState<CreateTransactionDto>({
    description: '',
    amount: 0,
    date: new Date().toISOString().split('T')[0],
    type: TransactionType.Despesa,
    categoryId: 0,
    personId: 0
  });

  // Estados para edição
  const [editandoTransacao, setEditandoTransacao] = useState<Transaction | null>(null);
  const [dadosEdicao, setDadosEdicao] = useState<UpdateTransactionDto>({});

  /**
   * Carrega os dados iniciais ao montar o componente
   */
  useEffect(() => {
    loadData();
  }, []);

  /**
   * Carrega todas as transações, pessoas e categorias
   */
  const loadData = async () => {
    try {
      setLoading(true);
      setErro('');
      
      // Carrega todos os dados em paralelo para melhor performance
      const [transacoesData, pessoasData, categoriasData] = await Promise.all([
        listTransactions(),
        listPeople(),
        listCategories()
      ]);
      
      setTransacoes(transacoesData);
      setPessoas(pessoasData);
      setCategorias(categoriasData);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao carregar dados');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função para lidar com mudanças nos campos do formulário
   */
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    
    setNovaTransacao(prev => ({
      ...prev,
      [name]: 
        name === 'amount' ? parseFloat(value) || 0 :
        name === 'categoryId' || name === 'personId' || name === 'type' ? parseInt(value) || 0 :
        value
    }));
  };

  /**
   * Verifica se a pessoa selecionada é menor de idade
   * @returns true se for menor de idade (< 18 anos)
   */
  const isSelectedPersonMinor = (): boolean => {
    const pessoa = pessoas.find(p => p.id === novaTransacao.personId);
    return pessoa ? pessoa.age < 18 : false;
  };

  /**
   * Filtra as categorias disponíveis baseado no tipo de transação selecionado
   * Regra: Se tipo for Despesa, só mostra categorias com finalidade Despesa ou Ambas
   *        Se tipo for Receita, só mostra categorias com finalidade Receita ou Ambas
   * @returns Array de categorias compatíveis
   */
  const getCompatibleCategories = (): Category[] => {
    return categorias.filter(categoria => {
      if (novaTransacao.type === TransactionType.Despesa) {
        return categoria.purpose === Purpose.Despesa || 
               categoria.purpose === Purpose.Ambas;
      } else {
        return categoria.purpose === Purpose.Receita || 
               categoria.purpose === Purpose.Ambas;
      }
    });
  };

  /**
   * Valida as regras de negócio antes de criar a transação
   * @returns true se válido, false caso contrário
   */
  const validateTransaction = (): boolean => {
    // Validação: Menor de idade só pode ter despesas
    if (isSelectedPersonMinor() && novaTransacao.type === TransactionType.Receita) {
      setErro('Menores de idade (menos de 18 anos) só podem ter despesas registradas');
      return false;
    }

    // Validação: Categoria deve ser compatível com o tipo
    const categoria = categorias.find(c => c.id === novaTransacao.categoryId);
    if (categoria) {
      if (novaTransacao.type === TransactionType.Despesa && categoria.purpose === Purpose.Receita) {
        setErro('Esta categoria só pode ser usada para receitas');
        return false;
      }
      if (novaTransacao.type === TransactionType.Receita && categoria.purpose === Purpose.Despesa) {
        setErro('Esta categoria só pode ser usada para despesas');
        return false;
      }
    }

    return true;
  };

  /**
   * Submete o formulário para criar uma nova transação
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Limpa mensagens
    setErro('');
    setSucesso('');

    // Validações básicas
    if (!novaTransacao.description.trim()) {
      setErro('Descrição é obrigatória');
      return;
    }

    if (novaTransacao.amount <= 0) {
      setErro('Valor deve ser maior que zero');
      return;
    }

    if (novaTransacao.personId === 0) {
      setErro('Selecione uma pessoa');
      return;
    }

    if (novaTransacao.categoryId === 0) {
      setErro('Selecione uma categoria');
      return;
    }

    // Validações de regras de negócio
    if (!validateTransaction()) {
      return;
    }

    try {
      setLoading(true);
      await createTransaction(novaTransacao);
      setSucesso('Transação criada com sucesso!');
      
      // Recarrega as transações
      const transacoesData = await listTransactions();
      setTransacoes(transacoesData);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao criar transação');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Obtém o nome da pessoa pelo ID
   */
  const getPersonName = (personId: number): string => {
    const pessoa = pessoas.find(p => p.id === personId);
    return pessoa ? pessoa.name : 'Desconhecido';
  };

  /**
   * Limpa todos os campos do formulário
   */
  const handleLimparFormulario = () => {
    setNovaTransacao({
      description: '',
      amount: 0,
      date: new Date().toISOString().split('T')[0],
      type: TransactionType.Despesa,
      categoryId: 0,
      personId: 0
    });
    setErro('');
    setSucesso('');
  };

  /**
   * Obtém a descrição da categoria pelo ID
   */
  const getCategoryDescription = (categoryId: number): string => {
    const categoria = categorias.find(c => c.id === categoryId);
    return categoria ? categoria.description : 'Desconhecida';
  };

  /**
   * Formata valor monetário para exibição
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
   * Calcula estatísticas das transações
   */
  const calculateStatistics = () => {
    const totalReceitas = transacoes
      .filter(t => t.type === TransactionType.Receita)
      .reduce((acc, t) => acc + t.amount, 0);
    
    const totalDespesas = transacoes
      .filter(t => t.type === TransactionType.Despesa)
      .reduce((acc, t) => acc + t.amount, 0);
    
    const saldo = totalReceitas - totalDespesas;
    
    return { totalReceitas, totalDespesas, saldo };
  };

  /**
   * Inicia a edição de uma transação
   */
  const handleIniciarEdicao = (transacao: Transaction) => {
    setEditandoTransacao(transacao);
    setDadosEdicao({
      description: transacao.description,
      amount: transacao.amount,
      date: new Date(transacao.createdAt).toISOString().split('T')[0],
      type: transacao.type,
      categoryId: transacao.categoryId,
      personId: transacao.personId
    });
    setErro('');
    setSucesso('');
  };

  /**
   * Cancela a edição
   */
  const handleCancelarEdicao = () => {
    setEditandoTransacao(null);
    setDadosEdicao({});
    setErro('');
  };

  /**
   * Atualiza os campos do formulário de edição
   */
  const handleEdicaoChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    
    setDadosEdicao(prev => ({
      ...prev,
      [name]: 
        name === 'amount' ? parseFloat(value) || 0 :
        name === 'categoryId' || name === 'personId' || name === 'type' ? parseInt(value) || 0 :
        value
    }));
  };

  /**
   * Salva as alterações da transação editada
   */
  const handleSalvarEdicao = async () => {
    if (!editandoTransacao) return;

    try {
      setLoading(true);
      setErro('');
      
      await updateTransaction(editandoTransacao.id, dadosEdicao);
      setSucesso('Transação atualizada com sucesso!');
      
      setEditandoTransacao(null);
      setDadosEdicao({});
      
      const transacoesData = await listTransactions();
      setTransacoes(transacoesData);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao atualizar transação');
    } finally {
      setLoading(false);
    }
  };

  const stats = calculateStatistics();

  return (
    <div className="transacoes-container">
      <h1>Gerenciamento de Transações</h1>

      {/* Mensagens */}
      {erro && <div className="mensagem erro">{erro}</div>}
      {sucesso && <div className="mensagem sucesso">{sucesso}</div>}

      {/* Estatísticas */}
      {transacoes.length > 0 && (
        <div className="stats-section">
          <div className="stat-card receita">
            <span className="stat-label">Total Receitas:</span>
            <span className="stat-value">{formatCurrency(stats.totalReceitas)}</span>
          </div>
          <div className="stat-card despesa">
            <span className="stat-label">Total Despesas:</span>
            <span className="stat-value">{formatCurrency(stats.totalDespesas)}</span>
          </div>
          <div className={`stat-card ${stats.saldo >= 0 ? 'saldo-positivo' : 'saldo-negativo'}`}>
            <span className="stat-label">Saldo:</span>
            <span className="stat-value">{formatCurrency(stats.saldo)}</span>
          </div>
        </div>
      )}

      {/* Formulário */}
      <div className="formulario-section">
        <h2>Registrar Nova Transação</h2>
        
        {/* Avisos importantes */}
        {pessoas.length === 0 && (
          <div className="mensagem aviso">
            Você precisa cadastrar pelo menos uma pessoa antes de criar transações.
          </div>
        )}
        
        {categorias.length === 0 && (
          <div className="mensagem aviso">
            Você precisa cadastrar pelo menos uma categoria antes de criar transações.
          </div>
        )}

        <form onSubmit={handleSubmit} className="formulario">
          <div className="form-group">
            <label htmlFor="personId">Pessoa:</label>
            <select
              id="personId"
              name="personId"
              value={novaTransacao.personId}
              onChange={handleInputChange}
              disabled={loading || pessoas.length === 0}
              required
            >
              <option value={0}>Selecione uma pessoa</option>
              {pessoas.map(pessoa => (
                <option key={pessoa.id} value={pessoa.id}>
                  {pessoa.name} - {pessoa.age} anos
                  {pessoa.age < 18 ? ' (Menor de idade)' : ''}
                </option>
              ))}
            </select>
            {isSelectedPersonMinor() && (
              <small className="form-hint aviso">
                Esta pessoa é menor de idade. Apenas despesas podem ser registradas.
              </small>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="type">Tipo:</label>
            <select
              id="type"
              name="type"
              value={novaTransacao.type}
              onChange={handleInputChange}
              disabled={loading}
              required
            >
              <option value={TransactionType.Despesa}>Despesa</option>
              <option value={TransactionType.Receita}>
                Receita {isSelectedPersonMinor() ? '(Não disponível para menores)' : ''}
              </option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="categoryId">Categoria:</label>
            <select
              id="categoryId"
              name="categoryId"
              value={novaTransacao.categoryId}
              onChange={handleInputChange}
              disabled={loading || categorias.length === 0}
              required
            >
              <option value={0}>Selecione uma categoria</option>
              {getCompatibleCategories().map(categoria => (
                <option key={categoria.id} value={categoria.id}>
                  {categoria.description} ({categoria.purpose})
                </option>
              ))}
            </select>
            <small className="form-hint">
              Apenas categorias compatíveis com o tipo selecionado são exibidas
            </small>
          </div>

          <div className="form-group">
            <label htmlFor="description">Descrição:</label>
            <textarea
              id="description"
              name="description"
              value={novaTransacao.description}
              onChange={handleInputChange}
              placeholder="Ex: Compra no supermercado, Pagamento salário..."
              rows={3}
              disabled={loading}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="amount">Valor (R$):</label>
            <input
              type="number"
              id="amount"
              name="amount"
              value={novaTransacao.amount || ''}
              onChange={handleInputChange}
              placeholder="0.00"
              step="0.01"
              min="0.01"
              disabled={loading}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="date">Data:</label>
            <input
              type="date"
              id="date"
              name="date"
              value={novaTransacao.date}
              onChange={handleInputChange}
              disabled={loading}
              required
            />
          </div>

          <div className="form-actions">
            <button 
              type="submit" 
              className="btn btn-primary" 
              disabled={loading || pessoas.length === 0 || categorias.length === 0}
            >
              {loading ? 'Salvando...' : 'Registrar Transação'}
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

      {/* Lista de transações */}
      <div className="lista-section">
        <h2>Transações Registradas</h2>
        {loading && <p>Carregando...</p>}
        
        {!loading && transacoes.length === 0 && (
          <p className="mensagem-vazia">Nenhuma transação registrada ainda.</p>
        )}

        {!loading && transacoes.length > 0 && (
          <div className="tabela-wrapper">
            <table className="tabela">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Pessoa</th>
                  <th>Tipo</th>
                  <th>Categoria</th>
                  <th>Descrição</th>
                  <th>Valor</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {transacoes.map((transacao) => (
                  <tr key={transacao.id}>
                    <td>{transacao.id}</td>
                    <td>{getPersonName(transacao.personId)}</td>
                    <td>
                      <span className={`badge ${
                        transacao.type === TransactionType.Receita 
                          ? 'badge-success' 
                          : 'badge-danger'
                      }`}>
                        {getTransactionTypeName(transacao.type)}
                      </span>
                    </td>
                    <td>{getCategoryDescription(transacao.categoryId)}</td>
                    <td>{transacao.description}</td>
                    <td className={
                      transacao.type === TransactionType.Receita 
                        ? 'valor-positivo' 
                        : 'valor-negativo'
                    }>
                      {transacao.type === TransactionType.Receita ? '+' : '-'}
                      {formatCurrency(transacao.amount)}
                    </td>
                    <td>
                      <button
                        className="btn btn-primary btn-small"
                        onClick={() => handleIniciarEdicao(transacao)}
                        disabled={loading}
                      >
                        Editar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal de Edição */}
      {editandoTransacao && (
        <div className="modal-overlay" onClick={handleCancelarEdicao}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Editar Transação</h2>
            
            <div className="form-group">
              <label htmlFor="edit-personId">Pessoa:</label>
              <select
                id="edit-personId"
                name="personId"
                value={dadosEdicao.personId || ''}
                onChange={handleEdicaoChange}
                disabled={loading}
              >
                <option value="">Selecione uma pessoa</option>
                {pessoas.map(pessoa => (
                  <option key={pessoa.id} value={pessoa.id}>
                    {pessoa.name} - {pessoa.age} anos
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="edit-type">Tipo:</label>
              <select
                id="edit-type"
                name="type"
                value={dadosEdicao.type ?? ''}
                onChange={handleEdicaoChange}
                disabled={loading}
              >
                <option value={TransactionType.Despesa}>Despesa</option>
                <option value={TransactionType.Receita}>Receita</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="edit-categoryId">Categoria:</label>
              <select
                id="edit-categoryId"
                name="categoryId"
                value={dadosEdicao.categoryId || ''}
                onChange={handleEdicaoChange}
                disabled={loading}
              >
                <option value="">Selecione uma categoria</option>
                {categorias.map(categoria => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.description}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="edit-description">Descrição:</label>
              <textarea
                id="edit-description"
                name="description"
                value={dadosEdicao.description || ''}
                onChange={handleEdicaoChange}
                rows={3}
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="edit-amount">Valor (R$):</label>
              <input
                type="number"
                id="edit-amount"
                name="amount"
                value={dadosEdicao.amount || ''}
                onChange={handleEdicaoChange}
                step="0.01"
                min="0.01"
                disabled={loading}
              />
            </div>

            <div className="form-group">
              <label htmlFor="edit-date">Data:</label>
              <input
                type="date"
                id="edit-date"
                name="date"
                value={dadosEdicao.date || ''}
                onChange={handleEdicaoChange}
                disabled={loading}
              />
            </div>

            <div className="form-actions">
              <button 
                className="btn btn-primary" 
                onClick={handleSalvarEdicao}
                disabled={loading}
              >
                {loading ? 'Salvando...' : 'Salvar'}
              </button>
              <button 
                className="btn btn-secondary" 
                onClick={handleCancelarEdicao}
                disabled={loading}
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default Transacoes;
