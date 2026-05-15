using Microsoft.AspNetCore.Mvc;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
}