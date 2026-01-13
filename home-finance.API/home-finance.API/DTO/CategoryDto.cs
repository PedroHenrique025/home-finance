using home_finance.API.Models;
using System.ComponentModel.DataAnnotations;

namespace home_finance.API.DTO
{
    /// <summary>
    /// Dto de resposta contendo os dados da categoria das Transações
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Identificador único da categoria.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        /// <example>Alimentação</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria: 0 = Despesa, 1 = Receita, 2 = Ambas.
        /// </summary>
        /// <example>0</example>
        public CategoryPurpose Purpose { get; set; }

        /// <summary>
        /// Descrição textual da finalidade.
        /// </summary>
        public string PurposeDescription { get; set; } = string.Empty;


    }

    /// <summary>
    /// DTO para criação de uma nova categoria.
    /// </summary>
    public class CreateCategoryDto
    {
        /// <summary>
        /// Descrição da categoria.
        /// </summary>
        /// <example>Alimentação</example>
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria: 0 = Despesa, 1 = Receita, 2 = Ambas.
        /// </summary>
        /// <example>0</example>
        [Required(ErrorMessage = "A finalidade é obrigatória")]
        public CategoryPurpose Purpose { get; set; }
    }
}
