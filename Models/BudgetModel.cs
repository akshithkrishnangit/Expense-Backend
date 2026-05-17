using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Budgets")]
public class BudgetModel
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("UserId")]
    public int UserId { get; set; }

    [Column("TotalBudget")]
    public decimal TotalBudget { get; set; }
}