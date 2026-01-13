using home_finance.API.DTO;
using home_finance.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace home_finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        /// <summary>
        /// Construtor com injeção de dependência do serviço de transações.
        /// </summary>
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Lista todas as transações cadastradas no sistema.
        /// </summary>
        /// <returns>Lista de transações ordenadas por data (mais recentes primeiro).</returns>
        /// <response code="200">Retorna a lista de transações com sucesso.</response>
        /// <remarks>
        /// Exemplo de resposta:
        /// 
        ///     GET /api/transactions
        ///     [
        ///       {
        ///         "id": 1,
        ///         "description": "Compra de supermercado",
        ///         "amount": 150.50,
        ///         "date": "2026-01-08T10:30:00",
        ///         "type": 0,
        ///         "categoryId": 2,
        ///         "categoryDescription": "Alimentação",
        ///         "personId": 1,
        ///         "personName": "João da Silva",
        ///         "createdAt": "2026-01-08T10:30:00"
        ///       }
        ///     ]
        /// </remarks>

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
        {
            var transactions = await _transactionService.GetAllAsync();
            return Ok(transactions);
        }


        /// <summary>
        /// Retorna a transação de acordo com o ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound(new { message = $"Transação com ID {id} não encontrada." });
            }
            return Ok(transaction);
        }

        /// <summary>
        /// Cria uma nova transação financeira.
        /// </summary>
        /// <param name="dto">Dados da transação a ser criada.</param>
        /// <returns>Transação criada com todos os dados.</returns>
        /// <response code="201">Transação criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou violação de regra de negócio.</response>
        /// <remarks>
        /// **REGRAS DE NEGÓCIO APLICADAS:**
        /// 
        /// 1. **Menores de idade (menor de 18 anos)**: Só podem registrar **DESPESAS** (type = 0).
        ///    - Se tentar criar uma receita, retorna erro 400.
        /// 
        /// 2. **Compatibilidade de categoria**: A categoria deve ser compatível com o tipo de transação:
        ///    - Categorias com finalidade "Despesa" (0): Só podem ser usadas em despesas.
        ///    - Categorias com finalidade "Receita" (1): Só podem ser usadas em receitas.
        ///    - Categorias com finalidade "Ambas" (2): Podem ser usadas em qualquer tipo.
        /// 
        /// 3. **Valor positivo**: O valor da transação deve ser sempre maior que zero.
        /// 
        /// 4. **Pessoa e categoria devem existir**: IDs informados devem estar cadastrados no sistema.
        /// 
        /// ---
        /// 
        /// **Exemplo de requisição válida (Receita):**
        /// 
        ///     POST /api/transactions
        ///     {
        ///        "description": "Salário mensal",
        ///        "amount": 5000.00,
        ///        "date": "2026-01-08",
        ///        "type": 1,
        ///        "categoryId": 2,
        ///        "personId": 1
        ///     }
        /// 
        /// **Exemplo de requisição válida (Despesa):**
        /// 
        ///     POST /api/transactions
        ///     {
        ///        "description": "Compra de supermercado",
        ///        "amount": 150.50,
        ///        "date": "2026-01-08",
        ///        "type": 0,
        ///        "categoryId": 3,
        ///        "personId": 2
        ///     }
        /// 
        /// **Exemplo de erro (menor de idade tentando criar receita):**
        /// 
        ///     {
        ///        "message": "A pessoa 'Maria Silva' tem 16 anos e é menor de idade. Menores de idade (menor de 18 anos) só podem registrar despesas."
        ///     }
        /// 
        /// **Exemplo de erro (categoria incompatível):**
        /// 
        ///     {
        ///        "message": "A categoria 'Salário' com finalidade 'Receita' não pode ser usada para despesas."
        ///     }
        /// </remarks>

        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionDto dto)
        {
            try
            {
                var transaction = await _transactionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
            }
            catch (ArgumentException ex)
            {
                // Erros de validação básica (pessoa/categoria não encontrada, valor inválido)
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Erros de regra de negócio (menor de idade, categoria incompatível)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Erro inesperado
                return StatusCode(500, new { message = "Erro interno ao criar transação.", details = ex.Message });
            }
        }


        /// <summary>
        /// Atualiza uma transação existente (atualização parcial).
        /// </summary>
        /// <param name="id">ID da transação.</param>
        /// <param name="dto">Dados a serem atualizados.</param>
        /// <returns>Transação atualizada.</returns>
        /// <response code="200">Transação atualizada com sucesso.</response>
        /// <response code="404">Transação não encontrada.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> Update(int id, [FromBody] UpdateTransactionDto dto)
        {
            try
            {
                var transaction = await _transactionService.UpdateAsync(id, dto);
                return Ok(transaction);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao atualizar transação.", details = ex.Message });

            }


        }


        /// <summary>
        /// Deleta uma única Transação.
        /// </summary>
        /// <param name="id">ID da Transação.</param>
        /// <returns>Confirmação da operação.</returns>
        /// <response code="204">Transação deletada com sucesso.</response>
        /// <response code="404">Transação não encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _transactionService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Transação com Id {id} não encontrada." });

            return NoContent();
        }

    }
}

