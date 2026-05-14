namespace TransactionSystemApi.DTOs;

public class GetTransactionRequest
{
    public string Id { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly TransactionDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public decimal exchangeRate { get; set; }
    
    public decimal ConvertedAmount { get; set; }
}