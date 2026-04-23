using FINANCETRACKER.Data;
using FINANCETRACKER.Models; // Importing the namespace where ExpenseModel is defined
using Microsoft.AspNetCore.Mvc;



namespace FINANCETRACKER.Controllers
{


    [ApiController]  //this class is used for API
    [Route("api/expense")] //base URL So your API starts with:
    public class ExpenseController : ControllerBase //creating a controller that handles api request 
    {
        // private static List<ExpenseModel> expenses = new List<ExpenseModel>(); //temperory database
        private readonly AppDbContext _context;
        private static decimal totalSalary = 5000; // example salary

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        //[HttpPost("add")] //defines api end point post is the type of request and add is the part of url
        //  public IActionResult AddExpense([FromBody] ExpenseModel req)
        //  { 

        //    return Ok(req.Name +"'s Expense added Succesfully");
        // }
        //[HttpPost("add")]
        //public IActionResult AddExpense([FromBody] ExpenseModel req)
        //{
        //    expenses.Add(req);
        //    return Ok(new
        //    {
        //       message = req.Name + "'s added successfully"
        //    });
        //}
        [HttpPost("add")]
        public IActionResult AddExpense([FromBody] ExpenseModel req)
        {
            _context.Expenses.Add(req);
            _context.SaveChanges();

            return Ok(new { message = "Expense added successfully" });
        }
        //[HttpGet("list")]
        //public IActionResult GetExpenses()
        //{
        //    return Ok(expenses);
        //}
        [HttpGet("list")]
        public IActionResult GetExpenses()
        {
            return Ok(_context.Expenses.ToList());
        }
        //  [HttpGet("total")]
        // [HttpGet("summary")]
        //public IActionResult GetSummary()
        //{
        //    decimal totalExpenses = expenses.Sum(e => e.Amount);
        //    decimal remainingSalary = totalSalary - totalExpenses;

        //    return Ok(new
        //    {
        //        TotalSalary = totalSalary,
        //        TotalSpent = totalExpenses,
        //        Remaining = remainingSalary
        //    });
        //}
        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            var expenses = _context.Expenses.ToList();
            var budget = _context.Budgets.FirstOrDefault();

            decimal totalBudget = budget?.TotalBudget ?? 0;
            decimal totalExpenses = expenses.Sum(e => e.Amount);
            decimal remainingSalary = totalBudget - totalExpenses;

            return Ok(new
            {
                totalBudget,
                totalSpent = totalExpenses,
                remaining = remainingSalary
            });
        }
        [HttpPost("set-budget")]
        public IActionResult SetBudget([FromBody] BudgetModel req)
        {
            var existing = _context.Budgets.FirstOrDefault();

            if (existing == null)
            {
                _context.Budgets.Add(req);
            }
            else
            {
                existing.TotalBudget = req.TotalBudget;
            }

            _context.SaveChanges();

            return Ok(new { message = "Budget saved" });
        }
        [HttpGet("budget")]
        public IActionResult GetBudget()
        {
            var budget = _context.Budgets.FirstOrDefault();

            return Ok(budget);
        }
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteExpense(int id)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id);

            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            _context.SaveChanges();

            return Ok(new { message = "Deleted successfully" });
        }
        [HttpPut("update/{id}")]
        public IActionResult UpdateExpense(int id, [FromBody] ExpenseModel req)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id);

            if (expense == null)
                return NotFound();

            expense.Name = req.Name;
            expense.Amount = req.Amount;
            expense.Category = req.Category;
            expense.Date = req.Date;

            _context.SaveChanges();

            return Ok(new { message = "Updated successfully" });
        }

    }
}
