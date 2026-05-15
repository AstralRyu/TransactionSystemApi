using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionSystemApi.Controllers;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Tests.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionService> _transactionService = new();
    private readonly TransactionsController _sut;

    public TransactionsControllerTests()
    {
        _sut = new TransactionsController(_transactionService.Object);
    }

    [Fact]
    public async Task CreateTransactionAsync_ValidRequest_ReturnsOkWithTransaction()
    {
        var cardId = Guid.NewGuid();
        var request = new CreateTransactionRequest
        {
            Amount = 75m,
            Description = "Fuel",
            Date = new DateOnly(2025, 4, 10),
            CardId = cardId
        };
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            CardId = cardId,
            Amount = 75m,
            Description = "Fuel",
            Date = new DateOnly(2025, 4, 10)
        };
        _transactionService.Setup(s => s.CreateTransactionAsync(request)).ReturnsAsync(transaction);

        var result = await _sut.CreateTransactionAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(transaction, ok.Value);
    }

    [Fact]
    public async Task GetTransactionsAsync_ValidRequest_ReturnsOkWithConvertedTransaction()
    {
        var transactionId = Guid.NewGuid();
        var converted = new ConvertedTransaction
        {
            Id = transactionId,
            CardId = Guid.NewGuid(),
            Amount = 100m,
            Description = "Hotel",
            Date = new DateOnly(2025, 5, 1),
            Currency = "Euro",
            ExchangeRate = 1.2m,
            ConvertedAmount = 120m
        };
        _transactionService.Setup(s => s.GetTransactionAsync(transactionId, "Euro"))
                           .ReturnsAsync(converted);

        var result = await _sut.GetTransactionsAsync(transactionId, "Euro");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(converted, ok.Value);
    }

    [Fact]
    public async Task GetTransactionsAsync_ServiceThrowsKeyNotFoundException_BubblesUp()
    {
        var transactionId = Guid.NewGuid();
        _transactionService.Setup(s => s.GetTransactionAsync(transactionId, "Euro"))
                           .ThrowsAsync(new KeyNotFoundException("Transaction not found."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetTransactionsAsync(transactionId, "Euro"));
    }

    [Fact]
    public async Task GetTransactionsAsync_ServiceThrowsInvalidOperationException_BubblesUp()
    {
        var transactionId = Guid.NewGuid();
        _transactionService.Setup(s => s.GetTransactionAsync(transactionId, "Euro"))
                           .ThrowsAsync(new InvalidOperationException("No exchange rate available."));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.GetTransactionsAsync(transactionId, "Euro"));
    }
}
