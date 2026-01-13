using home_finance.API.Models;
using System.ComponentModel.DataAnnotations;

namespace home_finance.API.DTO
{
    public class PersonDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        /// <summary>
        /// Lista de transações associadas à pessoa.
        /// Aqui utilizei uma Colletion ao invés de List para maior flexibilidade.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// booleano para verificar maioridade da pessoa.
        /// </summary>
        public bool IsMinor { get; set; }
    }

    /// <summary>
    /// DTO para mostrar pessoas e detalhes de suas transações.
    /// </summary>
    public class PersonDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsMinor { get; set; }

        public List<TransactionSummaryDto> Transactions { get; set; } = new();
    }

    /// <summary>
    /// DTO resumido para transações associadas a uma pessoa.
    /// </summary>
    public class TransactionSummaryDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para criação de uma nova pessoa.
    /// </summary>
    public class CreatePersonDto
    {
        /// <summary>
        /// Nome completo da pessoa.
        /// </summary>
        /// <example>João da Silva</example>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa (deve ser maior que 0 e menor que 150).
        /// </summary>
        /// <example>25</example>
        [Required(ErrorMessage = "A idade é obrigatória")]
        [Range(1, 150, ErrorMessage = "A idade deve estar entre 1 e 150 anos")]
        public int Age { get; set; }
    }

    /// <summary>
    /// DTO para atualização dos dados de uma pessoa.
    /// </summary>
    public class UpdatePersonDto
    {
        /// <summary>
        /// Nome completo (opcional na atualização).
        /// </summary>
        /// <example>João Silva Atualizado</example>
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string? Name { get; set; }

        /// <summary>
        /// Idade (opcional na atualização).
        /// </summary>
        /// <example>26</example>
        [Range(1, 150, ErrorMessage = "A idade deve estar entre 1 e 150 anos")]
        public int? Age { get; set; }
    }
}
