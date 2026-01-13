/**
 * Componente de gerenciamento de Pessoas
 * Permite listar, criar e deletar pessoas do sistema
 */

import React, { useState, useEffect } from 'react';
import { Person, CreatePersonDto, UpdatePersonDto } from '../types';
import { listPeople, createPerson, deletePerson, updatePerson } from '../services/personService';
import '../styles/Pessoas.css';

const Pessoas: React.FC = () => {
  // Estado para armazenar a lista de pessoas
  const [pessoas, setPessoas] = useState<Person[]>([]);
  
  // Estado para controlar loading durante operações
  const [loading, setLoading] = useState<boolean>(false);
  
  // Estado para mensagens de erro
  const [erro, setErro] = useState<string>('');
  
  // Estado para mensagens de sucesso
  const [sucesso, setSucesso] = useState<string>('');
  
  // Estado do formulário de nova pessoa
  const [novaPessoa, setNovaPessoa] = useState<CreatePersonDto>({
    name: '',
    age: 0
  });

  // Estados para edição
  const [editandoPessoa, setEditandoPessoa] = useState<Person | null>(null);
  const [dadosEdicao, setDadosEdicao] = useState<UpdatePersonDto>({});

  /**
   * useEffect para carregar a lista de pessoas ao montar o componente
   */
  useEffect(() => {
    loadPeople();
  }, []);

  /**
   * Função para carregar todas as pessoas do backend
   */
  const loadPeople = async () => {
    try {
      setLoading(true);
      setErro('');
      const data = await listPeople();
      setPessoas(data);
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao carregar pessoas');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função para lidar com mudanças nos campos do formulário
   * @param e - Evento de mudança do input
   */
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setNovaPessoa(prev => ({
      ...prev,
      // Converte para número se o campo for 'age', caso contrário mantém como string
      [name]: name === 'age' ? parseInt(value) || 0 : value
    }));
  };

  /**
   * Função para submeter o formulário e criar uma nova pessoa
   * @param e - Evento de submit do formulário
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Limpa mensagens anteriores
    setErro('');
    setSucesso('');

    // Validações do formulário
    if (!novaPessoa.name.trim()) {
      setErro('Nome é obrigatório');
      return;
    }

    if (novaPessoa.age <= 0) {
      setErro('Idade deve ser um número positivo');
      return;
    }

    try {
      setLoading(true);
      await createPerson(novaPessoa);
      setSucesso('Pessoa criada com sucesso!');
      
      // Recarrega a lista de pessoas
      await loadPeople();
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao criar pessoa');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função para deletar uma pessoa e suas transações
   * @param id - ID da pessoa a ser deletada
   * @param nome - Nome da pessoa (para exibir na confirmação)
   */
  const handleDeletar = async (id: number, nome: string) => {
    // Confirmação antes de deletar
    if (!window.confirm(
      `Tem certeza que deseja deletar ${nome}?\n\nATENÇÃO: Todas as transações desta pessoa também serão apagadas!`
    )) {
      return;
    }

    try {
      setLoading(true);
      setErro('');
      setSucesso('');
      
      await deletePerson(id);
      setSucesso('Pessoa deletada com sucesso!');
      
      // Recarrega a lista de pessoas
      await loadPeople();
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao deletar pessoa');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Função para verificar se a pessoa é menor de idade
   * Útil para exibir informações adicionais na listagem
   * @param age - Idade da pessoa
   * @returns true se for menor de idade (< 18 anos)
   */
  const isMinor = (age: number): boolean => {
    return age < 18;
  };

  /**
   * Limpa todos os campos do formulário
   */
  const handleLimparFormulario = () => {
    setNovaPessoa({ name: '', age: 0 });
    setErro('');
    setSucesso('');
  };

  /**
   * Inicia a edição de uma pessoa
   */
  const handleIniciarEdicao = (pessoa: Person) => {
    setEditandoPessoa(pessoa);
    setDadosEdicao({
      name: pessoa.name,
      age: pessoa.age
    });
    setErro('');
    setSucesso('');
  };

  /**
   * Cancela a edição
   */
  const handleCancelarEdicao = () => {
    setEditandoPessoa(null);
    setDadosEdicao({});
    setErro('');
  };

  /**
   * Atualiza os campos do formulário de edição
   */
  const handleEdicaoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setDadosEdicao(prev => ({
      ...prev,
      [name]: name === 'age' ? parseInt(value) || 0 : value
    }));
  };

  /**
   * Salva as alterações da pessoa editada
   */
  const handleSalvarEdicao = async () => {
    if (!editandoPessoa) return;

    try {
      setLoading(true);
      setErro('');
      
      await updatePerson(editandoPessoa.id, dadosEdicao);
      setSucesso('Pessoa atualizada com sucesso!');
      
      setEditandoPessoa(null);
      setDadosEdicao({});
      
      await loadPeople();
    } catch (error) {
      setErro(error instanceof Error ? error.message : 'Erro ao atualizar pessoa');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="pessoas-container">
      <h1>Gerenciamento de Pessoas</h1>

      {/* Mensagens de erro e sucesso */}
      {erro && <div className="mensagem erro">{erro}</div>}
      {sucesso && <div className="mensagem sucesso">{sucesso}</div>}

      {/* Formulário para criar nova pessoa */}
      <div className="formulario-section">
        <h2>Cadastrar Nova Pessoa</h2>
        <form onSubmit={handleSubmit} className="formulario">
          <div className="form-group">
            <label htmlFor="nome">Nome:</label>
            <input
              type="text"
              id="nome"
              name="name"
              value={novaPessoa.name}
              onChange={handleInputChange}
              placeholder="Digite o nome completo"
              disabled={loading}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="idade">Idade:</label>
            <input
              type="number"
              id="idade"
              name="age"
              value={novaPessoa.age || ''}
              onChange={handleInputChange}
              placeholder="Digite a idade"
              min="0"
              disabled={loading}
              required
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Salvando...' : 'Cadastrar Pessoa'}
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

      {/* Lista de pessoas cadastradas */}
      <div className="lista-section">
        <h2>Pessoas Cadastradas</h2>
        {loading && <p>Carregando...</p>}
        
        {!loading && pessoas.length === 0 && (
          <p className="mensagem-vazia">Nenhuma pessoa cadastrada ainda.</p>
        )}

        {!loading && pessoas.length > 0 && (
          <table className="tabela">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Idade</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map((pessoa) => (
                <tr key={pessoa.id}>
                  <td>{pessoa.id}</td>
                  <td>{pessoa.name}</td>
                  <td>{pessoa.age}</td>
                  <td>
                    {isMinor(pessoa.age) ? (
                      <span className="badge badge-warning">
                        Menor de Idade (apenas despesas)
                      </span>
                    ) : (
                      <span className="badge badge-success">Maior de Idade</span>
                    )}
                  </td>
                  <td>
                    <button
                      className="btn btn-primary btn-small"
                      onClick={() => handleIniciarEdicao(pessoa)}
                      disabled={loading}
                      style={{ marginRight: '0.5rem' }}
                    >
                      Editar
                    </button>
                    <button
                      className="btn btn-danger btn-small"
                      onClick={() => handleDeletar(pessoa.id, pessoa.name)}
                      disabled={loading}
                    >
                      Deletar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Modal de Edição */}
      {editandoPessoa && (
        <div className="modal-overlay" onClick={handleCancelarEdicao}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Editar Pessoa</h2>
            <div className="form-group">
              <label htmlFor="edit-name">Nome:</label>
              <input
                type="text"
                id="edit-name"
                name="name"
                value={dadosEdicao.name || ''}
                onChange={handleEdicaoChange}
                placeholder="Digite o nome completo"
                disabled={loading}
              />
            </div>
            <div className="form-group">
              <label htmlFor="edit-age">Idade:</label>
              <input
                type="number"
                id="edit-age"
                name="age"
                value={dadosEdicao.age || ''}
                onChange={handleEdicaoChange}
                placeholder="Digite a idade"
                min="0"
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

export default Pessoas;
