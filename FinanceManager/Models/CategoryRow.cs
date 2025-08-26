using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class CategoryRow
    {
        public int Id { get; }
        public string CategoryName { get; }
        public string TypeText { get; }
        public string AmountText { get; }
        public string? BudgetText {  get; }
        public bool HasBudget => !string.IsNullOrEmpty(BudgetText);

        public  CategoryRow(int id, string categoryName, string typeText, string amountText, string? budgetText)
        {
            Id = id;
            TypeText = typeText;
            CategoryName = categoryName;
            AmountText = amountText;
            BudgetText = budgetText;
        }
    }
}
