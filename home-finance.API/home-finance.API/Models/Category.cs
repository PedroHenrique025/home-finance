namespace home_finance.API.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Define a finalidade da categoria: 0 = Despesa, 1 = Receita, 2 = Ambas.
        /// </summary>
        public CategoryPurpose Purpose { get; set; }

        /// <summary>
        /// Descrição textual da finalidade.
        /// Obs:Optei por colocar uma descrição e um Enum para categoria, para facilitar a leitura e manutenção do código.
        /// Além de poupar tempo no front-end, evitando a necessidade de mapear os valores do Enum para strings.
        /// </summary>
        public string PurposeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Lista de transações associadas à categoria.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// Verifica se a categoria pode ser usada para o tipo de transação especificado.
        /// </summary>
        /// <param name="transactionType">Tipo da transação a ser validada.</param>
        /// <returns>True se a categoria é compatível com o tipo de transação.</returns>
        public bool IsValidForTransactionType(TransactionType transactionType)
        {
            return Purpose switch
            {
                CategoryPurpose.Both => true,
                CategoryPurpose.Expense => transactionType == TransactionType.Expense,
                CategoryPurpose.Income => transactionType == TransactionType.Income,
                _ => false
            };
        }
    }

    /// <summary>
    /// Enum que define as finalidades possíveis para uma categoria.
    /// </summary>
    public enum CategoryPurpose
    {

        Expense = 0,
        Income = 1,
        Both = 2
    }
}