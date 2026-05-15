using Moq;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Infrastructure;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Tests.Services;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _cardRepo = new();
    private readonly Mock<ITransactionRepository> _transactionRepo = new();
    private readonly Mock<ITreasuryApiClient> _client = new();
    private readonly CardService _sut;

    public CardServiceTests()
    {
        _sut = new CardService(_cardRepo.Object, _transactionRepo.Object, _client.Object);
    }

    [Fact]
    public async Task CreateCardAsync_ValidRequest_ReturnsCard()
    {
        var request = new CreateCardRequest { CreditLimit = 1000m };
        _cardRepo.Setup(r => r.AddCardAsync(It.IsAny<Card>()))
                 .ReturnsAsync((Card c) => c);

        var result = await _sut.CreateCardAsync(request);

        Assert.Equal(1000m, result.CreditLimit);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateCardAsync_ZeroCreditLimit_ThrowsArgumentException()
    {
        var request = new CreateCardRequest { CreditLimit = 0 };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCardAsync(request));
    }

    [Fact]
    public async Task CreateCardAsync_NegativeCreditLimit_ThrowsArgumentException()
    {
        var request = new CreateCardRequest { CreditLimit = -500m };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCardAsync(request));
    }

    [Fact]
    public async Task GetCardBalanceAsync_CardNotFound_ThrowsKeyNotFoundException()
    {
        _cardRepo.Setup(r => r.GetCardById(It.IsAny<Guid>())).ReturnsAsync((Card?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetCardBalanceAsync(Guid.NewGuid(), "Euro"));
    }

    [Fact]
    public async Task GetCardBalanceAsync_NoTransactions_ReturnsConvertedCreditLimit()
    {
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, CreditLimit = 1000m };

        _cardRepo.Setup(r => r.GetCardById(cardId)).ReturnsAsync(card);
        _transactionRepo.Setup(r => r.GetTransactionsByCardIdAsync(cardId))
                        .ReturnsAsync(new List<Transaction>());
        _client.Setup(c => c.GetLatestRateByCurrencyAsync("Euro")).ReturnsAsync(1.1m);

        var result = await _sut.GetCardBalanceAsync(cardId, "Euro");

        Assert.Equal(1100m, result);
    }

    [Fact]
    public async Task GetCardBalanceAsync_WithTransactions_ReturnsRemainingBalanceConverted()
    {
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, CreditLimit = 1000m };
        var transactions = new List<Transaction>
        {
            new() { Id = Guid.NewGuid(), CardId = cardId, Amount = 200m, Date = DateOnly.FromDateTime(DateTime.Today), Description = "tx1" },
            new() { Id = Guid.NewGuid(), CardId = cardId, Amount = 100m, Date = DateOnly.FromDateTime(DateTime.Today), Description = "tx2" }
        };

        _cardRepo.Setup(r => r.GetCardById(cardId)).ReturnsAsync(card);
        _transactionRepo.Setup(r => r.GetTransactionsByCardIdAsync(cardId)).ReturnsAsync(transactions);
        _client.Setup(c => c.GetLatestRateByCurrencyAsync("Euro")).ReturnsAsync(2m);

        var result = await _sut.GetCardBalanceAsync(cardId, "Euro");

        // balance = 1000 - 300 = 700, converted = 700 * 2 = 1400
        Assert.Equal(1400m, result);
    }

    [Fact]
    public async Task GetCardBalanceAsync_RoundsToTwoDecimalPlaces()
    {
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, CreditLimit = 100m };

        _cardRepo.Setup(r => r.GetCardById(cardId)).ReturnsAsync(card);
        _transactionRepo.Setup(r => r.GetTransactionsByCardIdAsync(cardId))
                        .ReturnsAsync(new List<Transaction>());
        _client.Setup(c => c.GetLatestRateByCurrencyAsync("Euro")).ReturnsAsync(1.123456m);

        var result = await _sut.GetCardBalanceAsync(cardId, "Euro");

        Assert.Equal(Math.Round(100m * 1.123456m, 2), result);
    }
}
