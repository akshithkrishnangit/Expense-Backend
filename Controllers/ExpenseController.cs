using FINANCETRACKER.Data;
using FINANCETRACKER.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace FINANCETRACKER.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expense")]
    public class ExpenseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------- ADD EXPENSE --------------------
        [HttpPost("add")]
        public IActionResult AddExpense([FromBody] ExpenseModel req)
        {
            // FIX UTC ISSUE
            req.Date = DateTime.SpecifyKind(req.Date, DateTimeKind.Utc);

            _context.Expenses.Add(req);
            _context.SaveChanges();

            return Ok(new { message = "Expense added successfully" });
        }

        // -------------------- GET EXPENSES (USER WISE) --------------------
        [HttpGet("list/{userId}")]
        public IActionResult GetExpenses(int userId)
        {
            var data = _context.Expenses
                .Where(e => e.UserId == userId)
                .ToList();

            return Ok(data);
        }

        // -------------------- UPDATE EXPENSE --------------------
        [HttpPut("update/{id}")]
        public IActionResult UpdateExpense(int id, [FromBody] ExpenseModel req)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.Id == id);

            if (expense == null)
                return NotFound();

            expense.Name = req.Name;
            expense.Amount = req.Amount;
            expense.Category = req.Category;

            // FIX UTC ISSUE
            expense.Date = DateTime.SpecifyKind(req.Date, DateTimeKind.Utc);

            _context.SaveChanges();

            return Ok(new { message = "Updated successfully" });
        }

        // -------------------- DELETE EXPENSE --------------------
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

        // -------------------- SET BUDGET (USER WISE) --------------------
        [HttpPost("set-budget")]
        public IActionResult SetBudget([FromBody] BudgetModel req)
        {
            var existing = _context.Budgets
                .FirstOrDefault(b => b.UserId == req.UserId);

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

        // -------------------- GET BUDGET (USER WISE) --------------------
        [HttpGet("budget/{userId}")]
        public IActionResult GetBudget(int userId)
        {
            var budget = _context.Budgets
                .FirstOrDefault(b => b.UserId == userId);

            return Ok(budget);
        }

        // -------------------- SUMMARY (DASHBOARD) --------------------
        [HttpGet("summary/{userId}")]
        public IActionResult GetSummary(int userId)
        {
            var expenses = _context.Expenses
                .Where(e => e.UserId == userId)
                .ToList();

            var budget = _context.Budgets
                .FirstOrDefault(b => b.UserId == userId);

            decimal totalBudget = budget?.TotalBudget ?? 0;
            decimal totalSpent = expenses.Sum(e => e.Amount);
            decimal remaining = totalBudget - totalSpent;

            return Ok(new
            {
                totalBudget,
                totalSpent,
                remaining
            });
        }
    }
}