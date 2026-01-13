using home_finance.API.Data;
using home_finance.API.DTO;
using home_finance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace home_finance.API.Services
{
    /// <summary>
    /// Serviço responsável pela lógica de negócio das pessoas.
    /// </summary>
    public class PersonService: IPersonService
    {
        private readonly AppDbContext _context;
        public PersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonDto>> GetAllAsync()
        {
            var person = await _context.People
                .Include(p => p.Transactions)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return person.Select(MapToDto);

        }

        public async Task<PersonDetailDto> GetDetailByIdAsync(int id)
        {
            var person = await _context.People
                .Include(p => p.Transactions)    
                .ThenInclude(t => t.Category)  
                .FirstOrDefaultAsync(p => p.Id == id); 

            return person != null ? MapToDetailDto(person) : null;
        }
        
        /// <summary>
        /// Retorna uma pessoa específica por ID.
        /// </summary>
        public async Task<PersonDto?> GetByIdAsync(int id)
        {
            var person = await _context.People.FindAsync(id);
            return person != null ? MapToDto(person) : null;
        }

        /// <summary>
        /// Retorna apenas menores de idade (menor de 18 anos).
        /// </summary>
        public async Task<IEnumerable<PersonDto>> GetMinorsAsync()
        {
            var minors = await _context.People
                .Where(p => p.Age < 18)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return minors.Select(MapToDto);
        }

        /// <summary>
        /// Retorna apenas maiores de idade (18 anos ou mais).
        /// </summary>
        public async Task<IEnumerable<PersonDto>> GetAdultsAsync()
        {
            var adults = await _context.People
                .Where(p => p.Age >= 18)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return adults.Select(MapToDto);
        }

        /// <summary>
        /// Cria uma nova pessoa fazendo as validações requisitadas.
        /// </summary>
        public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
        {
            // Validação: Nome não pode ser vazio
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("O nome não pode ser vazio.");
            }

            // Coloquei um limite mínimo e máximo de idade para evitar dados inválidos
            if (dto.Age <= 0 || dto.Age > 150)
            {
                throw new ArgumentException("A idade deve estar entre 1 e 150 anos.");
            }

            // Utilizando o método para normalizar o nome e evitar duplicatas por variações de maiúsculas/minúsculas
            var normalizedName = NormalizeName(dto.Name);

            // Verificação de duplicidade de nome
            var exists = await _context.People
                .AnyAsync(p => p.Name.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Já existe uma pessoa cadastrada com o nome '{normalizedName}'.");
            }

            // Criar pessoa
            var person = new Person
            {
                Name = normalizedName,
                Age = dto.Age
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return MapToDto(person);
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa existente.
        /// Aqui, assim como os outros métodos de update, utilizei a atualização parcial para maior flexibilidade.
        /// </summary>
        public async Task<PersonDto?> UpdateAsync(int id, UpdatePersonDto dto)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return null;

            // Atualizar nome (se fornecido)
            if (dto.Name != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    throw new ArgumentException("O nome não pode ser vazio.");
                }

                var normalizedName = NormalizeName(dto.Name);

                // Verificar duplicata (exceto a própria pessoa)
                var exists = await _context.People
                    .AnyAsync(p => p.Id != id && p.Name.ToLower() == normalizedName.ToLower());

                if (exists)
                {
                    throw new InvalidOperationException(
                        $"Já existe outra pessoa com o nome '{normalizedName}'.");
                }

                person.Name = normalizedName;
            }

            // Atualizar idade (se fornecido)
            if (dto.Age.HasValue)
            {
                if (dto.Age.Value <= 0 || dto.Age.Value > 150)
                {
                    throw new ArgumentException("A idade deve estar entre 1 e 150 anos.");
                }

                person.Age = dto.Age.Value;
            }

            await _context.SaveChangesAsync();
            return MapToDto(person);
        }

        /// <summary>
        /// Deleta uma pessoa e todas suas transações em cascata.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var person = await _context.People
                .Include(p => p.Transactions) // Carregar transações para informação
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                return false;

            // A lógica de cascade delete está configurada no DbContext
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Mapeia Person para PersonDto.
        /// </summary>
        private static PersonDto MapToDto(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age,
                IsMinor = person.IsMinor
            };
        }

        private static PersonDetailDto MapToDetailDto(Person person)
        {
            return new PersonDetailDto
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age,
                IsMinor = person.IsMinor,
                Transactions = person.Transactions
                    .Select(t => new TransactionSummaryDto
                    {
                        Id = t.Id,
                        Amount = t.Amount,
                        Date = t.Date,
                        Description = t.Description
                    })
                    .ToList()
            };
        }


        /// <summary>
        /// Optei por normalizar o nome da pessoa utilizando Title Case.
        /// </summary>
        private static string NormalizeName(string name)
        {
            name = name.Trim();

            // Converte para Title Case (João Da Silva)
            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }
    }
}
