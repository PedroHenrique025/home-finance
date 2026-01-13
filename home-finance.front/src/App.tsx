/**
 * Componente principal App
 * Configura as rotas e o layout geral da aplicação
 * Gerencia a navegação entre as diferentes páginas do sistema
 */

import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom';
import Pessoas from './components/Pessoas';
import Categorias from './components/Categorias';
import './styles/App.css';
import Transacoes from './components/Transacoes';
import TotaisPorPessoa from './components/TotaisPorPessoa';
import TotaisPorCategoria from './components/TotaisPorCategoria';
import './styles/App.css';

/**
 * Componente de Layout com navegação
 * Inclui o menu de navegação e a área de conteúdo
 */
const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const location = useLocation();

  /**
   * Verifica se a rota atual está ativa
   * @param path - Caminho da rota
   * @returns true se a rota está ativa
   */
  const isActive = (path: string): boolean => {
    return location.pathname === path;
  };

  return (
    <div className="app-layout">
      {/* Header com navegação */}
      <header className="app-header">
        <div className="header-content">
          <Link to="/" className="logo">
            <span className="logo-text">Controle Financeiro</span>
          </Link>
          
          <nav className="main-nav">
            <Link 
              to="/" 
              className={`nav-link ${isActive('/') ? 'active' : ''}`}
            >
              Pessoas
            </Link>
            <Link 
              to="/categorias" 
              className={`nav-link ${isActive('/categorias') ? 'active' : ''}`}
            >
              Categorias
            </Link>
            <Link 
              to="/transacoes" 
              className={`nav-link ${isActive('/transacoes') ? 'active' : ''}`}
            >
              Transações
            </Link>
            <Link 
              to="/totais-pessoa" 
              className={`nav-link ${isActive('/totais-pessoa') ? 'active' : ''}`}
            >
              Totais por Pessoa
            </Link>
            <Link 
              to="/totais-categoria" 
              className={`nav-link ${isActive('/totais-categoria') ? 'active' : ''}`}
            >
              Totais por Categoria
            </Link>
          </nav>
        </div>
      </header>

      {/* Área de conteúdo principal */}
      <main className="app-main">
        {children}
      </main>

      {/* Footer */}
      <footer className="app-footer">
        <p>
          © 2026 Sistema de Controle de Gastos Residenciais | 
          Desenvolvido com React & TypeScript
        </p>
      </footer>
    </div>
  );
};

/**
 * Componente principal da aplicação
 * Configura o roteador e define todas as rotas do sistema
 */
const App: React.FC = () => {
  return (
    <Router>
      <Layout>
        <Routes>
          {/* Rota da página inicial - abre direto em Pessoas */}
          <Route path="/" element={<Pessoas />} />
          
          {/* Rotas de cadastro */}
          <Route path="/categorias" element={<Categorias />} />
          <Route path="/transacoes" element={<Transacoes />} />
          
          {/* Rotas de consulta/relatórios */}
          <Route path="/totais-pessoa" element={<TotaisPorPessoa />} />
          <Route path="/totais-categoria" element={<TotaisPorCategoria />} />
          
          {/* Rota 404 - Página não encontrada */}
          <Route path="*" element={
            <div className="pagina-404">
              <h1>404</h1>
              <h2>Página não encontrada</h2>
              <p>A página que você está procurando não existe.</p>
              <Link to="/" className="btn btn-primary">
                Voltar para Pessoas
              </Link>
            </div>
          } />
        </Routes>
      </Layout>
    </Router>
  );
};

export default App;
