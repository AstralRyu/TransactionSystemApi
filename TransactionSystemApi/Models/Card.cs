namespace TransactionSystemApi.Models;

public class Card
{
    public Guid Id { get; private set; }
    
    public decimal Balance { get; set; }
    
}