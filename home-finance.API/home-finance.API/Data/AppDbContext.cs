using home_finance.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace home_finance.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Person
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Age).IsRequired();
            });

            // Configuração da entidade Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Description).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Purpose).IsRequired();
            });

            // Configuração da entidade Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Description).IsRequired().HasMaxLength(500);
                entity.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(t => t.Type).IsRequired();
                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("datetime('now')")
                    .ValueGeneratedOnAdd();
                entity.Property(t => t.UpdateAt)
                    .ValueGeneratedOnAddOrUpdate();

                // Relacionamento com Person: ao deletar pessoa, deleta transações (cascade)
                entity.HasOne(t => t.Person)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(t => t.PersonId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relacionamento com Category
                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Description = "Remuneração", Purpose = CategoryPurpose.Income },
                new Category { Id = 2, Description = "Mantimentos", Purpose = CategoryPurpose.Expense },
                new Category { Id = 3, Description = "Contas", Purpose = CategoryPurpose.Expense },
                new Category { Id = 4, Description = "Entretenimento", Purpose = CategoryPurpose.Expense },
                new Category { Id = 5, Description = "Outros", Purpose = CategoryPurpose.Both }
            );

            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Pedro", Age = 20 },
                new Person { Id = 2, Name = "Nathalia", Age = 19 },
                new Person { Id = 3, Name = "Isac", Age = 16 },
                new Person { Id = 4, Name = "Luiz", Age = 30 },
                new Person { Id = 5, Name = "Paulo", Age = 60 },
                new Person { Id = 6, Name = "Dielle", Age = 23 }
            );

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction { Id = 1, Description = "Salário Pedro", Amount = 5000.00m, Type = TransactionType.Income, PersonId = 1, CategoryId = 1 },
                new Transaction { Id = 2, Description = "Salário Nathalia", Amount = 1000.00m, Type = TransactionType.Income, PersonId = 2, CategoryId = 1 },
                new Transaction { Id = 3, Description = "Comidas do Mês", Amount = 1000.00m, Type = TransactionType.Expense, PersonId = 1, CategoryId = 2 },
                new Transaction { Id = 4, Description = "Produtos de Higiene", Amount = 200.00m, Type = TransactionType.Expense, PersonId = 2, CategoryId = 2 },
                new Transaction { Id = 5, Description = "Conta de luz", Amount = 80.50m, Type = TransactionType.Expense, PersonId = 1, CategoryId = 3 },
                new Transaction { Id = 6, Description = "Cinema", Amount = 100.00m, Type = TransactionType.Expense, PersonId = 1, CategoryId = 4 },
                new Transaction { Id = 7, Description = "Troca de Pneu do Carro", Amount = 2000.00m, Type = TransactionType.Expense, PersonId = 1, CategoryId = 5 },
                new Transaction { Id = 8, Description = "Remédios para Paulo", Amount = 750.00m, Type = TransactionType.Expense, PersonId = 1, CategoryId = 2 },
                new Transaction { Id = 9, Description = "Bebidas do Churrasco", Amount = 400.00m, Type = TransactionType.Expense, PersonId = 6, CategoryId = 5 }

            );
        }
    }
}