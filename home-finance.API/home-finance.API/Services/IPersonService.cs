using home_finance.API.DTO;

namespace home_finance.API.Services
{
    /// <summary>
    /// Interface que define o contrato para operações de pessoas.
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Retorna todas as pessoas cadastradas.
        /// </summary>
        Task<IEnumerable<PersonDto>> GetAllAsync();

        /// <summary>
        /// Retorna todas as pessoas com suas transações.
        /// </summary>
        Task<PersonDetailDto?> GetDetailByIdAsync(int id);


        /// <summary>
        /// Retorna uma pessoa específica por ID.
        /// </summary>
        Task<PersonDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova pessoa.
        /// </summary>
        Task<PersonDto> CreateAsync(CreatePersonDto dto);

        // <summary>
        /// Atualiza os dados de uma pessoa existente.
        /// </summary>
        Task<PersonDto?> UpdateAsync(int id, UpdatePersonDto dto);

        /// <summary>
        /// Deleta uma pessoa e todas suas transações em cascata.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Retorna apenas pessoas menores de idade.
        /// </summary>
        Task<IEnumerable<PersonDto>> GetMinorsAsync();

        /// <summary>
        /// Retorna apenas pessoas maiores de idade.
        /// </summary>
        Task<IEnumerable<PersonDto>> GetAdultsAsync();

    }
}
