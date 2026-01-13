using home_finance.API.Data;
using home_finance.API.DTO;
using home_finance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace home_finance.API.Services
{
    /// <summary>
    /// Serviço responsável pela lógica de negócio das categorias.
    /// Implementa as validações e operações relacionadas às categorias.
    /// </summary>
    public class CategoryService: ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as categorias cadastradas ordenadas por descrição.
        /// </summary>
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Description)
                .ToListAsync();

            return categories.Select(MapToDto);
        }

        /// <summary>
        /// Retorna uma categoria específica por ID.
        /// </summary>
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        /// <summary>
        /// Retorna categorias filtradas por finalidade.
        /// </summary>
        public async Task<IEnumerable<CategoryDto>> GetByPurposeAsync(CategoryPurpose purpose)
        {
            var categories = await _context.Categories
                .Where(c => c.Purpose == purpose)
                .OrderBy(c => c.Description)
                .ToListAsync();

            return categories.Select(MapToDto);
        }

        /// <summary>
        /// Cria uma nova categoria aplicando as regras de negócio.
        /// Validações na descrição
        /// Prevenção de duplicidade
        /// </summary>
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            // Validação: Descrição não pode ser vazia
            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("A descrição da categoria não pode ser vazia.");
            }

            // Validação: Verificar se já existe categoria com mesma descrição
            var exists = await _context.Categories
                .AnyAsync(c => c.Description.ToLower() == dto.Description.ToLower());

            if (exists)
            {
                throw new InvalidOperationException(
                    $"Já existe uma categoria com a descrição '{dto.Description}'.");
            }

            // Criar categoria
            var category = new Category
            {
                Description = dto.Description.Trim(),
                Purpose = dto.Purpose
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        /// <summary>
        /// Mapeia uma entidade Category para CategoryDto.
        /// </summary>
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Description = category.Description,
                Purpose = category.Purpose,
                PurposeDescription = GetPurposeDescription(category.Purpose)
            };
        }

        /// <summary>
        /// Retorna uma descrição com base no tipo da finalidade.
        /// </summary>
        private static string GetPurposeDescription(CategoryPurpose purpose)
        {
            return purpose switch
            {
                CategoryPurpose.Expense => "Despesa",
                CategoryPurpose.Income => "Receita",
                CategoryPurpose.Both => "Ambas",
                _ => "Desconhecida"
            };
        }
        
    }
}
