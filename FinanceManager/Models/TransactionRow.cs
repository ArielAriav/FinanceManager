namespace FinanceManager.Models;

public record TransactionRow
{
    public int Id { get; init; }
    public string TypeText { get; init; } = "";
    public string CategoryName { get; init; } = "";
    public string AmountText { get; init; } = "";
    public string WhenText { get; init; } = "";
}
