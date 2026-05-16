using Microsoft.EntityFrameworkCore;
using FINANCETRACKER.Models;

namespace FINANCETRACKER.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseModel>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2); // 18 digits total, 2 after decimal
            modelBuilder.Entity<BudgetModel>()
                .Property(b => b.TotalBudget)
                .HasPrecision(18, 2); // 18 digits total, 2 after decimal

        }

        public DbSet<ExpenseModel> Expenses { get; set; }
        public DbSet<BudgetModel> Budgets { get; set; }
        public DbSet<UserModel> USERS { get; set; }
    }
}