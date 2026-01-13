using home_finance.API.Data;
using home_finance.API.DTO;
using home_finance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace home_finance.API.Services
{
    /// <summary>
    /// Serviço responsável pela lógica de negócio das transações.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        //Construtor Inicializando o contexto do banco de dados
        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todas as transações com dados relacionados (Person e Category).
        /// </summary>
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .ToListAsync();

            return transactions.Select(MapToDto);
        }

        /// <summary>
        /// Retorna uma transação específica por ID.
        /// </summary>
        public async Task<TransactionDto?> GetByIdAsync(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            return transaction != null ? MapToDto(transaction) : null;
        }

        /// <summary>
        /// Cria uma nova transação aplicando as regras de negócio:
        /// Menor de idade só pode criar despesas.
        /// Categoria deve ser compatível com o tipo de transação.
        /// Valor deve ser positivo.
        /// </summary>
        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            // Validação: valor deve ser positivo
            if (dto.Amount <= 0)
            {
                throw new ArgumentException("O valor da transação deve ser positivo.");
            }

            // Buscar pessoa para validar idade
            var person = await _context.People.FindAsync(dto.PersonId);
            if (person == null)
            {
                throw new ArgumentException("Pessoa não encontrada.");
            }

            // Regra de negócio: menor de idade só pode criar despesas
            if (person.IsMinor && dto.Type == TransactionType.Income)
            {
                throw new InvalidOperationException(
                    "Menores de idade (menor de 18 anos) só podem registrar despesas.");
            }

            // Buscar categoria para validar compatibilidade
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Categoria não encontrada.");
            }

            // Implementação da regra de negócio: categoria ser compatível com tipo de transação
            if (!category.IsValidForTransactionType(dto.Type))
            {
                throw new InvalidOperationException(
                    $"A categoria '{category.Description}' não pode ser usada para {(dto.Type == TransactionType.Expense ? "despesas" : "receitas")}.");
            }

            // Criar a transação
            var transaction = new Transaction
            {
                Description = dto.Description,
                Amount = dto.Amount,
                Date = dto.Date,
                Type = dto.Type,
                CategoryId = dto.CategoryId,
                PersonId = dto.PersonId,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Recarregar com dados relacionados
            await _context.Entry(transaction)
                .Reference(t => t.Person)
                .LoadAsync();
            await _context.Entry(transaction)
                .Reference(t => t.Category)
                .LoadAsync();

            return MapToDto(transaction);
        }

        /// <summary>
        /// Atualiza uma transação existente.
        /// </summary>
        public async Task<TransactionDto?> UpdateAsync(int id, UpdateTransactionDto dto)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Person)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return null;

            // Atualizar apenas campos fornecidos (partial update)
            if (dto.Description != null)
                transaction.Description = dto.Description;

            if (dto.Amount.HasValue)
            {
                if (dto.Amount.Value <= 0)
                    throw new ArgumentException("O valor deve ser positivo.");
                transaction.Amount = dto.Amount.Value;
            }

            if (dto.Date.HasValue)
                transaction.Date = dto.Date.Value;

            if (dto.Type.HasValue)
            {
                // Validar regra de menor de idade
                if (transaction.Person.IsMinor && dto.Type.Value == TransactionType.Income)
                    throw new InvalidOperationException("Menor de idade só pode registrar despesas.");

                transaction.Type = dto.Type.Value;
            }

            if (dto.CategoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Categoria não encontrada.");

                if (!category.IsValidForTransactionType(transaction.Type))
                    throw new InvalidOperationException("Categoria incompatível com o tipo de transação.");

                transaction.CategoryId = dto.CategoryId.Value;
            }

            transaction.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }


        /// <summary>
        /// Deleta uma transação existente.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }


        #region Relatórios (Totais)
        /// <summary>
        /// Retorna relatório de totais por pessoa com consolidado geral.
        /// Optei por fazer os cálculos todos aqui no back-end.
        /// Mais segurança e garantia do fluxo correto dos dados e também uma facilidade maior para fazer qualquer mudança nessa rotina sem afetar a construção do front
        /// </summary>
        public async Task<PersonTotalsReportDto> GetPersonTotalsReportAsync()
        {
            // Buscar todas as pessoas com suas transações
            var people = await _context.People
                .Include(p => p.Transactions)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var report = new PersonTotalsReportDto();

            // Calcular totais para cada pessoa
            foreach (var person in people)
            {
                // Cálculo de receitas (Type = 1)
                var totalIncome = person.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                // Cálculo de despesas (Type = 0)
                var totalExpense = person.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                // Cálculo do saldo
                var balance = totalIncome - totalExpense;

                // Adicionar pessoa ao relatório
                report.People.Add(new PersonTotalsDto
                {
                    PersonId = person.Id,
                    PersonName = person.Name,
                    Age = person.Age,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = balance
                });

                // Acumular totais gerais
                report.GrandTotalIncome += totalIncome;
                report.GrandTotalExpense += totalExpense;
            }

            // Calcular saldo geral
            report.GrandBalance = report.GrandTotalIncome - report.GrandTotalExpense;

            return report;
        }

        /// <summary>
        /// Retorna relatório de totais por categoria com consolidado geral.
        /// </summary>
        public async Task<CategoryTotalsReportDto> GetCategoryTotalsReportAsync()
        {
            // Buscar todas as categorias com suas transações
            var categories = await _context.Categories
                .Include(c => c.Transactions)
                .OrderBy(c => c.Description)
                .ToListAsync();

            var report = new CategoryTotalsReportDto();

            // Calcular totais para cada categoria
            foreach (var category in categories)
            {
                // Cálculo de receitas (Type = 1)
                var totalIncome = category.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                // Cálculo de despesas (Type = 0)
                var totalExpense = category.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                // Cálculo do saldo
                var balance = totalIncome - totalExpense;

                // Mapear finalidade da categoria
                var purposeDescription = category.Purpose switch
                {
                    CategoryPurpose.Expense => "Despesa",
                    CategoryPurpose.Income => "Receita",
                    CategoryPurpose.Both => "Ambas",
                    _ => "Desconhecida"
                };

                // Adicionar categoria ao relatório
                report.Categories.Add(new CategoryTotalsDto
                {
                    CategoryId = category.Id,
                    CategoryName = category.Description,
                    Purpose = purposeDescription,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = balance
                });

                // Acumular totais gerais
                report.GrandTotalIncome += totalIncome;
                report.GrandTotalExpense += totalExpense;
            }

            // Calcular saldo geral
            report.GrandBalance = report.GrandTotalIncome - report.GrandTotalExpense;

            return report;
        }
        #endregion
        
        /// <summary>
        /// Mapeia uma entidade Transaction para TransactionDto.
        /// </summary>
        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Type = transaction.Type,
                CategoryId = transaction.CategoryId,
                CategoryDescription = transaction.Category?.Description ?? string.Empty,
                PersonId = transaction.PersonId,
                PersonName = transaction.Person?.Name ?? string.Empty,
                CreatedAt = transaction.CreatedAt
            };
        }
    }
}