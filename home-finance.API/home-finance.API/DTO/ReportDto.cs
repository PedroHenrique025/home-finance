namespace home_finance.API.DTO
{
    /// <summary>
    /// DTO para totais individuais de uma pessoa.
    /// Optei por criar um DTO específico para os Totais
    /// Mais facilidade para trabalhar e fazer a manutenção e testes dessa maneira
    /// </summary>
    public class PersonTotalsDto
    {

        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public int Age { get; set; }

        /// <summary>
        /// Total de receitas da pessoa.
        /// </summary>
        public decimal TotalIncome { get; set; }

        /// <summary>
        /// Total de despesas da pessoa.
        /// </summary>
        public decimal TotalExpense { get; set; }

        /// <summary>
        /// Saldo (Receitas - Despesas).
        /// </summary>
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// DTO para relatório consolidado de totais por pessoa.
    /// </summary>
    public class PersonTotalsReportDto
    {
        /// <summary>
        /// Lista de totais por pessoa.
        /// </summary>
        public List<PersonTotalsDto> People { get; set; } = new();

        /// <summary>
        /// Total geral de receitas (soma de todas as pessoas).
        /// </summary>
        public decimal GrandTotalIncome { get; set; }

        /// <summary>
        /// Total geral de despesas (soma de todas as pessoas).
        /// </summary>
        public decimal GrandTotalExpense { get; set; }

        /// <summary>
        /// Saldo geral (Total Receitas - Total Despesas).
        /// </summary>
        public decimal GrandBalance { get; set; }
    }

    /// <summary>
    /// DTO para totais individuais de uma categoria.
    /// </summary>
    public class CategoryTotalsDto
    {
        /// <summary>
        /// ID da categoria.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Nome da categoria.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria.
        /// </summary>
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Total de receitas da categoria.
        /// </summary>
        public decimal TotalIncome { get; set; }

        /// <summary>
        /// Total de despesas da categoria.
        /// </summary>
        public decimal TotalExpense { get; set; }

        /// <summary>
        /// Saldo (Receitas - Despesas).
        /// </summary>
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// DTO para relatório consolidado de totais por categoria.
    /// </summary>
    public class CategoryTotalsReportDto
    {
        /// <summary>
        /// Lista de totais por categoria.
        /// </summary>
        public List<CategoryTotalsDto> Categories { get; set; } = new();

        /// <summary>
        /// Total geral de receitas (soma de todas as categorias).
        /// </summary>
        public decimal GrandTotalIncome { get; set; }

        /// <summary>
        /// Total geral de despesas (soma de todas as categorias).
        /// </summary>
        public decimal GrandTotalExpense { get; set; }

        /// <summary>
        /// Saldo geral (Total Receitas - Total Despesas).
        /// </summary>
        public decimal GrandBalance { get; set; }
    }
}
