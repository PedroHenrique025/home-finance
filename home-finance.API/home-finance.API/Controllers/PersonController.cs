using home_finance.API.DTO;
using home_finance.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace home_finance.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento http de pessoas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;
        /// <summary>
        /// Construtor com injeção de dependência do serviço de pessoas.
        /// </summary>
        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Lista todas as pessoas cadastradas no sistema.
        /// </summary>
        /// <returns>Lista de pessoas ordenadas por nome.</returns>
        /// <response code="200">Retorna a lista de pessoas com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAll()
        {
            var people = await _personService.GetAllAsync();
            return Ok(people);
        }

        /// <summary>
        /// Lista todas as pessoas cadastradas com detalhes de suas transações.
        /// </summary>
        /// <returns> Lista de pessoas detalhadas ordenadas por nome</returns>
        /// <response code="200">Retorna a lista de pessoas com sucesso</response>
        [HttpGet("{id}/detail")]
        [ProducesResponseType(typeof(IEnumerable<PersonDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonDetailDto>> GetByIdDetail(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "O ID da pessoa deve ser maior que zero." });
            }

            var person = await _personService.GetDetailByIdAsync(id);

            if (person == null)
            {
                return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });
            }

            return Ok(person);
        }

        /// <summary>
        /// Obtém uma pessoa específica por ID.
        /// </summary>
        /// <param name="id">ID da pessoa.</param>
        /// <returns>Dados completos da pessoa.</returns>
        /// <response code="200">Pessoa encontrada com sucesso.</response>
        /// <response code="404">Pessoa não encontrada.</response>
        /// <response code="400">ID inválido.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonDto>> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "O ID da pessoa deve ser maior que zero." });
            }

            var person = await _personService.GetByIdAsync(id);

            if (person == null)
            {
                return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });
            }

            return Ok(person);
        }

        /// <summary>
        /// Lista apenas pessoas menores de idade (menor de 18 anos).
        /// </summary>
        /// <returns>Lista de menores de idade.</returns>
        /// <response code="200">Retorna a lista de menores.</response>
        [HttpGet("minors")]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetMinors()
        {
            var minors = await _personService.GetMinorsAsync();
            return Ok(minors);
        }

        /// <summary>
        /// Lista apenas pessoas maiores de idade (18 anos ou mais).
        /// </summary>
        /// <returns>Lista de maiores de idade.</returns>
        /// <response code="200">Retorna a lista de adultos.</response>
        [HttpGet("adults")]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAdults()
        {
            var adults = await _personService.GetAdultsAsync();
            return Ok(adults);
        }

        /// <summary>
        /// Cria uma nova pessoa.
        /// </summary>
        /// <param name="dto">Dados da pessoa a ser criada.</param>
        /// <returns>Pessoa criada.</returns>
        /// <response code="201">Pessoa criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou pessoa duplicada.</response>
        /// <remarks>
        /// **Regras de validação:**
        /// <ul>
        ///     <li>1. Nome é obrigatório (3-200 caracteres)</li>
        ///     <li>2. Idade entre 1 e 150 anos </li>
        ///     <li>3. Nome é único (case-insensitive)</li>
        ///     <li>4. Nome é normalizado automaticamente (Title Case)</li>
        /// </ul>
        /// **Exemplos:**
        /// 
        ///     POST /api/persons
        ///     {
        ///        "name": "João da Silva",
        ///        "age": 25
        ///     }
        ///     
        ///     POST /api/persons
        ///     {
        ///        "name": "Maria Santos",
        ///        "age": 16
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonDto>> Create([FromBody] CreatePersonDto dto)
        {
            try
            {
                var person = await _personService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma pessoa existente.
        /// Atualização parcial utilizando Patch ao invés de PUT
        /// </summary>
        /// <param name="id">ID da pessoa.</param>
        /// <param name="dto">Dados a serem atualizados.</param>
        /// <returns>Pessoa atualizada.</returns>
        /// <response code="200">Pessoa atualizada com sucesso.</response>
        /// <response code="404">Pessoa não encontrada.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonDto>> Update(int id, [FromBody] UpdatePersonDto dto)
        {
            try
            {
                var person = await _personService.UpdateAsync(id, dto);
                if (person == null)
                    return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });

                return Ok(person);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma pessoa e todas suas transações.
        /// </summary>
        /// <param name="id">ID da pessoa.</param>
        /// <returns>Confirmação da operação.</returns>
        /// <response code="204">Pessoa deletada com sucesso.</response>
        /// <response code="404">Pessoa não encontrada.</response>
        /// <remarks>
        /// 
        /// Ao deletar uma pessoa, todas as suas transações serão removidas permanentemente.
        /// 
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _personService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Pessoa com ID {id} não encontrada." });

            return NoContent();
        }
    }
}