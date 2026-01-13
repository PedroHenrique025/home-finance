using home_finance.API.Models;

namespace home_finance.API.DTO
{
    /// <summary>
    /// DTO de resposta contendo os dados da transação.
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Identificador único da transação.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação.
        /// </summary>
        /// <example>Compra de supermercado</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação.
        /// </summary>
        /// <example>150.50</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// Data de realização da transação.
        /// </summary>
        /// <example>2026-01-08T10:30:00</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tipo da transação (0 = Despesa, 1 = Receita).
        /// </summary>
        /// <example>0</example>
        public TransactionType Type { get; set; }

        /// <summary>
        /// ID da categoria associada.
        /// </summary>
        /// <example>2</example>
        public int CategoryId { get; set; }

        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        /// <example>Alimentação</example>
        public string CategoryDescription { get; set; } = string.Empty;

        /// <summary>
        /// ID da pessoa responsável.
        /// </summary>
        /// <example>1</example>
        public int PersonId { get; set; }

        /// <summary>
        /// Nome da pessoa responsável.
        /// </summary>
        /// <example>João da Silva</example>
        public string PersonName { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO para criação de uma nova transação.
    /// </summary>
    public class CreateTransactionDto
    {
        /// <summary>
        /// Descrição da transação.
        /// </summary>
        /// <example>Salário mensal</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação (deve ser positivo).
        /// </summary>
        /// <example>5000.00</example>
        public decimal Amount { get; set; }

        /// <summary>
        /// Data de realização da transação.
        /// </summary>
        /// <example>2026-01-08</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tipo da transação (0 = Despesa, 1 = Receita).
        /// </summary>
        /// <example>1</example>
        public TransactionType Type { get; set; }

        /// <summary>
        /// ID da categoria.
        /// </summary>
        /// <example>2</example>
        public int CategoryId { get; set; }

        /// <summary>
        /// ID da pessoa.
        /// </summary>
        /// <example>1</example>
        public int PersonId { get; set; }
    }

    /// <summary>
    /// DTO para editar uma transação existente.
    /// </summary>
    
    public class UpdateTransactionDto
    {
        /// <summary>
        /// Descrição da transação.
        /// </summary>
        /// <example>Salário mensal atualizado</example>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Valor da transação (deve ser positivo).
        /// </summary>
        /// <example>5500.00</example>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Data de realização da transação.
        /// </summary>
        /// <example>2026-01-10</example>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Tipo da transação (0 = Despesa, 1 = Receita).
        /// </summary>
        /// <example>1</example>
        public TransactionType? Type { get; set; }
        /// <summary>
        /// ID da categoria.
        /// </summary>
        /// <example>3</example>
        public int? CategoryId { get; set; }
        /// <summary>
        /// ID da pessoa.
        /// </summary>
        /// <example>1</example>
        public int PersonId { get; set; }
    }
}