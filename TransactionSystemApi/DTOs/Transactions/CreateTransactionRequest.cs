using System.ComponentModel.DataAnnotations;

namespace TransactionSystemApi.DTOs;

public class CreateTransactionRequest
{
    [Required]
    public string Description { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public Guid CardId { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    [Required]
    public decimal Amount { get; set; }
}