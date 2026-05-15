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
    public async Task<ActionResult<Transaction>> CreateTransactionAsync(
        [FromBody] CreateTransactionRequest transactionRequest)
    {
        var transaction = await _transactionService.CreateTransactionAsync(transactionRequest);
        return Ok(transaction);
    }

    [HttpGet("{transactionId:guid}/get")]
    public async Task<ActionResult<ConvertedTransaction>> GetTransactionsAsync(Guid transactionId,
        [FromQuery] string currency)
    {
        var convertedTransaction = await _transactionService.GetTransactionAsync(transactionId, currency);
        return Ok(convertedTransaction);
    }
}