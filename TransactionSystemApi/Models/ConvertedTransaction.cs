namespace TransactionSystemApi.Models;

public class ConvertedTransaction : Transaction
{
    public string Currency { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal ConvertedAmount { get; set; }
}