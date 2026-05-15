using Moq;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Infrastructure;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepo = new();
    private readonly Mock<ITreasuryApiClient> _client = new();
    private readonly TransactionService _sut;

    public TransactionServiceTests()
    {
        _sut = new TransactionService(_transactionRepo.Object, _client.Object);
    }

    [Fact]
    public async Task CreateTransactionAsync_ValidRequest_ReturnsTransaction()
    {
        var cardId = Guid.NewGuid();
        var date = new DateOnly(2025, 1, 15);
        var request = new CreateTransactionRequest
        {
            Amount = 250m,
            Description = "Groceries",
            Date = date,
            CardId = cardId
        };
        _transactionRepo.Setup(r => r.AddTransactionAsync(It.IsAny<Transaction>()))
                        .ReturnsAsync((Transaction t) => t);

        var result = await _sut.CreateTransactionAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(250m, result.Amount);
        Assert.Equal("Groceries", result.Description);
        Assert.Equal(date, result.Date);
        Assert.Equal(cardId, result.CardId);
    }

    [Fact]
    public async Task GetTransactionAsync_TransactionNotFound_ThrowsKeyNotFoundException()
    {
        _transactionRepo.Setup(r => r.GetTransactionByIdAsync(It.IsAny<Guid>()))
                        .ReturnsAsync((Transaction?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetTransactionAsync(Guid.NewGuid(), "Euro"));
    }

    [Fact]
    public async Task GetTransactionAsync_ValidTransaction_ReturnsConvertedTransaction()
    {
        var transactionId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var date = new DateOnly(2025, 3, 10);
        var transaction = new Transaction
        {
            Id = transactionId,
            CardId = cardId,
            Amount = 100m,
            Description = "Dinner",
            Date = date
        };

        _transactionRepo.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);
        _client.Setup(c => c.GetRateForDateByCurrencyAsync("Euro", date)).ReturnsAsync(1.5m);

        var result = await _sut.GetTransactionAsync(transactionId, "Euro");

        Assert.Equal(transactionId, result.Id);
        Assert.Equal(cardId, result.CardId);
        Assert.Equal(100m, result.Amount);
        Assert.Equal("Dinner", result.Description);
        Assert.Equal(date, result.Date);
        Assert.Equal("Euro", result.Currency);
        Assert.Equal(1.5m, result.ExchangeRate);
        Assert.Equal(150m, result.ConvertedAmount);
    }

    [Fact]
    public async Task GetTransactionAsync_RoundsConvertedAmountToTwoDecimalPlaces()
    {
        var transactionId = Guid.NewGuid();
        var date = new DateOnly(2025, 6, 1);
        var transaction = new Transaction
        {
            Id = transactionId,
            CardId = Guid.NewGuid(),
            Amount = 33.33m,
            Description = "Coffee",
            Date = date
        };

        _transactionRepo.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);
        _client.Setup(c => c.GetRateForDateByCurrencyAsync("Yen", date)).ReturnsAsync(3.333m);

        var result = await _sut.GetTransactionAsync(transactionId, "Yen");

        Assert.Equal(Math.Round(33.33m * 3.333m, 2), result.ConvertedAmount);
    }

    [Fact]
    public async Task GetTransactionAsync_PassesCorrectCurrencyAndDateToClient()
    {
        var transactionId = Guid.NewGuid();
        var date = new DateOnly(2024, 12, 25);
        var transaction = new Transaction
        {
            Id = transactionId,
            CardId = Guid.NewGuid(),
            Amount = 50m,
            Description = "Gift",
            Date = date
        };

        _transactionRepo.Setup(r => r.GetTransactionByIdAsync(transactionId)).ReturnsAsync(transaction);
        _client.Setup(c => c.GetRateForDateByCurrencyAsync("Dollar", date)).ReturnsAsync(1.0m);

        await _sut.GetTransactionAsync(transactionId, "Dollar");

        _client.Verify(c => c.GetRateForDateByCurrencyAsync("Dollar", date), Times.Once);
    }
}
