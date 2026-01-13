/**
 * Ponto de entrada da aplicação React
 * Renderiza o componente App no DOM
 */

import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/index.css';
import App from './App';

// Obtém o elemento root do HTML
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

// Renderiza a aplicação
// StrictMode ajuda a identificar problemas potenciais na aplicação
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
