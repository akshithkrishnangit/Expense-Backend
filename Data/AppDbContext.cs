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
            // ---------------- EXPENSES ----------------
            modelBuilder.Entity<ExpenseModel>(e =>
            {
                e.ToTable("Expenses");

                e.Property(x => x.Id).HasColumnName("Id");
                e.Property(x => x.UserId).HasColumnName("UserId");
                e.Property(x => x.Name).HasColumnName("Name");
                e.Property(x => x.Amount).HasColumnName("Amount");
                e.Property(x => x.Category).HasColumnName("Category");
                e.Property(x => x.Date).HasColumnName("Date");
            });

            // ---------------- BUDGETS ----------------
            modelBuilder.Entity<BudgetModel>(b =>
            {
                b.ToTable("Budgets");

                b.Property(x => x.Id).HasColumnName("Id");
                b.Property(x => x.UserId).HasColumnName("UserId");
                b.Property(x => x.TotalBudget).HasColumnName("TotalBudget");
            });

            modelBuilder.Entity<ExpenseModel>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BudgetModel>()
                .Property(b => b.TotalBudget)
                .HasPrecision(18, 2);
        }

        public DbSet<ExpenseModel> Expenses { get; set; }
        public DbSet<BudgetModel> Budgets { get; set; }
        public DbSet<UserModel> USERS { get; set; }
    }
}