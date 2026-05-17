using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FINANCETRACKER.Models
{
    public class ExpenseModel
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("USER_ID")]
        public int UserId { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("CATEGORY")]
        public string Category { get; set; }

        [Column("DATE")]
        public DateTime Date { get; set; }
    }
}