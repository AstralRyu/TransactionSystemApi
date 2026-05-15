namespace TransactionSystemApi.Models;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}