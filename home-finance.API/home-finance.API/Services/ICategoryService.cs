using home_finance.API.DTO;
using home_finance.API.Models;

namespace home_finance.API.Services
{
    /// <summary>
    /// Interface define o contrato para operações de transações financeiras
    /// Implementa as validações e operações relacionadas às categorias.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retorna todas as categorias cadastradas.
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetAllAsync();

        /// <summary>
        /// Retorna uma categoria por ID.
        ///</summary>
        Task<CategoryDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova categoria aplicando as regras de negócio.
        ///</summary>

        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

        /// <summary>
        /// Retorna categorias filtradas por finalidade.
        /// </summary>
        Task<IEnumerable<CategoryDto>> GetByPurposeAsync(CategoryPurpose purpose);


    }
}
