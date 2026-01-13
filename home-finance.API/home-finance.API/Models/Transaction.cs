using System;

namespace home_finance.API.Models
{
    public class Transaction
    {
        /// <summary>
        /// Identificador único da transação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação.
        /// </summary>
        /// <example>Salário mensal</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação.
        /// </summary>
        /// <example>5000.00</example>

        public decimal Amount { get; set; }

        /// <summary>
        /// Data da realização da Transação
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tipo da transação: 0 = Despesa, 1 = Receita.
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Identificadores relacionais e o objeto de cada um
        /// </summary>
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;

        /// <summary>
        /// Datas de Gerenciamento do registro
        /// Data de criação do registro e última atualização
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }

    /// <summary>
    /// Enum que define os tipos de transação possíveis.
    /// </summary>
    public enum TransactionType
    {
        Expense = 0,
        Income = 1
    }
}