using System.ComponentModel.DataAnnotations;

namespace FINANCETRACKER.Models
{
    public class BudgetModel
    {
        [Key]
        public int Id { get; set; }

        public decimal TotalBudget { get; set; }
    }
}
