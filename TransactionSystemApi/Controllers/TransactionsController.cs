using Microsoft.AspNetCore.Mvc;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Controllers;

[ApiController]
[Route("transaction")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionRequest transactionRequest)
    {        
        try
        {
            var transaction = await _transactionService.CreateTransactionAsync(transactionRequest);
            return Ok(transaction);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{transactionId:uuid}/get")]
    public async Task<ActionResult<ConvertedTransaction>> GetTransactionsAsync(Guid transactionId, [FromQuery] string currency)
    {
        try
        {
            var convertedTransaction = await _transactionService.GetTransactionAsync(transactionId, currency);
            return Ok(convertedTransaction);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
}