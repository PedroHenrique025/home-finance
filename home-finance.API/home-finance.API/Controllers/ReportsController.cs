using home_finance.API.DTO;
using home_finance.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace home_finance.API.Controllers
{
    /// <summary>
    /// Controller responsável por relatórios e consultas consolidadas.
    /// Optei por criar um controlador só para a parte de relatório para criar rotas e rotinas específicas para eles
    /// Achei a melhor opção por apresentar uma rotina diferente das demais, e assim tornando mais fácil fazer a manutenção desse código como se fosse um "módulo" separado
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public ReportsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Consulta de totais por pessoa.
        /// Lista todas as pessoas com total de receitas, despesas e saldo,
        /// além do consolidado geral.
        /// </summary>
        /// <returns>Relatório de totais por pessoa com consolidado geral.</returns>
        /// <response code="200">Relatório gerado com sucesso.</response>
        /// <remarks>
        /// Este endpoint retorna um relatório completo com:
        /// 
        /// Por pessoa:
        /// - Nome e idade
        /// - Total de receitas
        /// - Total de despesas
        /// - Saldo (receitas - despesas)
        /// 
        /// Consolidado geral:
        /// - Total geral de receitas (soma de todas as pessoas)
        /// - Total geral de despesas (soma de todas as pessoas)
        /// - Saldo geral (total receitas - total despesas)
        /// 
        /// Exemplo de resposta:
        /// 
        ///     GET /api/reports/persons
        ///     {
        ///       "people": [
        ///         {
        ///           "personId": 1,
        ///           "personName": "João Da Silva",
        ///           "age": 25,
        ///           "totalIncome": 5000.00,
        ///           "totalExpense": 2500.00,
        ///           "balance": 2500.00
        ///         },
        ///         {
        ///           "personId": 2,
        ///           "personName": "Maria Santos",
        ///           "age": 16,
        ///           "totalIncome": 0.00,
        ///           "totalExpense": 800.00,
        ///           "balance": -800.00
        ///         }
        ///       ],
        ///       "grandTotalIncome": 5000.00,
        ///       "grandTotalExpense": 3300.00,
        ///       "grandBalance": 1700.00
        ///     }
        /// </remarks>
        [HttpGet("persons")]
        [ProducesResponseType(typeof(PersonTotalsReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PersonTotalsReportDto>> GetPersonTotals()
        {
            var report = await _transactionService.GetPersonTotalsReportAsync();
            return Ok(report);
        }

        /// <summary>
        /// Consulta de totais por categoria.
        /// Lista todas as categorias com total de receitas, despesas e saldo,
        /// além do consolidado geral.
        /// </summary>
        /// <returns>Relatório de totais por categoria com consolidado geral.</returns>
        /// <response code="200">Relatório gerado com sucesso.</response>
        /// <remarks>
        /// Este endpoint retorna um relatório completo com:
        /// 
        /// Por categoria:
        /// - Nome e finalidade da categoria
        /// - Total de receitas
        /// - Total de despesas
        /// - Saldo (receitas - despesas)
        /// 
        /// Consolidado geral:
        /// - Total geral de receitas (soma de todas as categorias)
        /// - Total geral de despesas (soma de todas as categorias)
        /// - Saldo geral (total receitas - total despesas)
        /// 
        /// Exemplo de resposta:
        /// 
        ///     GET /api/reports/categories
        ///     {
        ///       "categories": [
        ///         {
        ///           "categoryId": 1,
        ///           "categoryName": "alimentação",
        ///           "purpose": "Despesa",
        ///           "totalIncome": 0.00,
        ///           "totalExpense": 1500.00,
        ///           "balance": -1500.00
        ///         },
        ///         {
        ///           "categoryId": 2,
        ///           "categoryName": "salário",
        ///           "purpose": "Receita",
        ///           "totalIncome": 8000.00,
        ///           "totalExpense": 0.00,
        ///           "balance": 8000.00
        ///         }
        ///       ],
        ///       "grandTotalIncome": 8000.00,
        ///       "grandTotalExpense": 1500.00,
        ///       "grandBalance": 6500.00
        ///     }
        /// </remarks>
        [HttpGet("categories")]
        [ProducesResponseType(typeof(CategoryTotalsReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CategoryTotalsReportDto>> GetCategoryTotals()
        {
            var report = await _transactionService.GetCategoryTotalsReportAsync();
            return Ok(report);
        }
    }
}
