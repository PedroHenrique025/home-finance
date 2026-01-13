using home_finance.API.DTO;
using home_finance.API.Models;
using home_finance.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace home_finance.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento http das categorias.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Construtor com injeção de dependência do serviço de categorias.
        /// </summary>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lista todas as categorias cadastradas no sistema.
        /// </summary>
        /// <returns>Lista de categorias ordenadas por descrição.</returns>
        /// <response code="200">Retorna a lista de categorias com sucesso.</response>
        /// <remarks>
        /// Exemplo de resposta:
        /// 
        ///     GET /api/categories
        ///     [
        ///       {
        ///         "id": 1,
        ///         "description": "Alimentação",
        ///         "purpose": 0,
        ///         "purposeDescription": "Despesa"
        ///       },
        ///       {
        ///         "id": 2,
        ///         "description": "Salário",
        ///         "purpose": 1,
        ///         "purposeDescription": "Receita"
        ///       }
        ///     ]
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Obtém uma categoria específica por ID.
        /// </summary>
        /// <param name="id">ID da categoria.</param>
        /// <returns>Dados completos da categoria.</returns>
        /// <response code="200">Categoria encontrada com sucesso.</response>
        /// <response code="404">Categoria não encontrada.</response>
        /// <response code="400">ID inválido (deve ser maior que zero).</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "O ID da categoria deve ser maior que zero." });
            }

            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = $"Categoria com ID {id} não encontrada." });
            }

            return Ok(category);
        }

        /// <summary>
        /// Lista categorias filtradas por finalidade.
        /// </summary>
        /// <param name="purpose">Finalidade: 0 = Despesa, 1 = Receita, 2 = Ambas.</param>
        /// <returns>Lista de categorias da finalidade especificada.</returns>
        /// <response code="200">Retorna a lista de categorias filtradas.</response>
        [HttpGet("purpose/{purpose}")]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByPurpose(CategoryPurpose purpose)
        {
            var categories = await _categoryService.GetByPurposeAsync(purpose);
            return Ok(categories);
        }

        /// <summary>
        /// Cria uma nova categoria.
        /// </summary>
        /// <param name="dto">Dados da categoria a ser criada.</param>
        /// <returns>Categoria criada.</returns>
        /// <response code="201">Categoria criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou categoria duplicada.</response>
        /// <remarks>
        /// **Regras de negócio:**
        /// 
        /// 1. **Descrição única**: Não pode haver duas categorias com a mesma descrição.
        /// 2. **Descrição obrigatória**: A descrição não pode ser vazia.
        /// 3. **Finalidade válida**: Deve ser 0 (Despesa), 1 (Receita) ou 2 (Ambas).
        /// 
        /// **Tipos de finalidade:**
        /// - **0 (Despesa)**: Categoria usada apenas para despesas
        /// - **1 (Receita)**: Categoria usada apenas para receitas
        /// - **2 (Ambas)**: Categoria pode ser usada tanto para despesas quanto receitas
        /// 
        /// ---
        /// 
        /// **Exemplo de requisição (Categoria de Despesa):**
        /// 
        ///     POST /api/categories
        ///     {
        ///        "description": "Alimentação",
        ///        "purpose": 0
        ///     }
        /// 
        /// **Exemplo de requisição (Categoria de Receita):**
        /// 
        ///     POST /api/categories
        ///     {
        ///        "description": "Salário",
        ///        "purpose": 1
        ///     }
        /// 
        /// **Exemplo de requisição (Categoria Ambas):**
        /// 
        ///     POST /api/categories
        ///     {
        ///        "description": "Outros",
        ///        "purpose": 2
        ///     }
        /// 
        /// **Exemplo de erro (Categoria duplicada):**
        /// 
        ///     {
        ///        "message": "Já existe uma categoria com a descrição 'Alimentação'."
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
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
                return StatusCode(500, new { message = "Erro interno ao criar categoria.", details = ex.Message });
            }
        }
    }
}