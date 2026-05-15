namespace TransactionSystemApi.DTOs;

public class GetTransactionResponse
{
    public Guid Id { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly TransactionDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public decimal ExchangeRate { get; set; }
    
    public decimal ConvertedAmount { get; set; }
    
    public string Currency { get; set; }
}