using home_finance.API.DTO;

namespace home_finance.API.Services
{
    /// <summary>
    /// Interface que define o contrato para operações de transações.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Retorna todas as transações.
        /// </summary>
        Task<IEnumerable<TransactionDto>> GetAllAsync();

        /// <summary>
        /// Retorna uma transação por ID.
        /// </summary>
        Task<TransactionDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova transação aplicando regras de negócio.
        /// </summary>
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto);

        /// <summary>
        /// Atualiza uma transação existente.
        ///</summary>
        Task<TransactionDto?> UpdateAsync(int id, UpdateTransactionDto dto);

        /// <summary>
        /// Deleta uma transação existente.
        ///</summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Retorna relatório de totais por pessoa.
        /// Calcula receitas, despesas e saldo de cada pessoa.
        /// </summary>
        /// <returns>Relatório com totais por pessoa e total geral.</returns>
        Task<PersonTotalsReportDto> GetPersonTotalsReportAsync();

        /// <summary>
        /// Retorna relatório de totais por categoria.
        /// Calcula receitas, despesas e saldo de cada categoria.
        /// </summary>
        /// <returns>Relatório com totais por categoria e total geral.</returns>
        Task<CategoryTotalsReportDto> GetCategoryTotalsReportAsync();
    }
}