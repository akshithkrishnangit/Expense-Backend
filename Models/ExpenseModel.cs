using System.ComponentModel.DataAnnotations;

namespace FINANCETRACKER.Models
{
    public class ExpenseModel
    {
        [Key] // optional but good practice
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
    }
}
