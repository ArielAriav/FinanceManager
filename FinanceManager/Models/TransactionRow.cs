namespace FinanceManager.Models
{
    public class TransactionRow
    {
        public int Id { get; }
        public string TypeText { get; }
        public string CategoryName { get; }
        public string AmountText { get; }
        public string WhenText { get; }

        public TransactionRow(int id, string typeText, string categoryName, string amountText, string whenText)
        {
            Id = id;
            TypeText = typeText;
            CategoryName = categoryName;
            AmountText = amountText;
            WhenText = whenText;
        }
    }
}
