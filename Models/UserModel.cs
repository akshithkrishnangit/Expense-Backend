namespace FINANCETRACKER.Models
{
    public class UserModel
    {
        public int ID { get; set; }

        public string? NAME { get; set; }

        public string? USERNAME { get; set; }

        public string? PASSWORD { get; set; }

        public DateTime? CREATED_DATE { get; set; }
    }
}