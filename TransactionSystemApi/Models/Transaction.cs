namespace TransactionSystemApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public Card Card { get; set; } = null!;
    public DateOnly TransactionDate { get; set; }
    public decimal Amount { get; set; }
}