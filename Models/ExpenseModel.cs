using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINANCETRACKER.Models
{
    [Table("Expenses")]
    public class ExpenseModel
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("Category")]
        public string Category { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }
    }
}