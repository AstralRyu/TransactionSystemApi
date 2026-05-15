using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionSystemApi.Controllers;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Tests.Controllers;

public class CardsControllerTests
{
    private readonly Mock<ICardService> _cardService = new();
    private readonly CardsController _sut;

    public CardsControllerTests()
    {
        _sut = new CardsController(_cardService.Object);
    }

    [Fact]
    public async Task CreateCard_ValidRequest_ReturnsOkWithCard()
    {
        var request = new CreateCardRequest { CreditLimit = 500m };
        var card = new Card { Id = Guid.NewGuid(), CreditLimit = 500m };
        _cardService.Setup(s => s.CreateCardAsync(request)).ReturnsAsync(card);

        var result = await _sut.CreateCard(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(card, ok.Value);
    }

    [Fact]
    public async Task CreateCard_ZeroCreditLimit_ReturnsBadRequest()
    {
        var request = new CreateCardRequest { CreditLimit = 0 };

        var result = await _sut.CreateCard(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateCard_NegativeCreditLimit_ReturnsBadRequest()
    {
        var request = new CreateCardRequest { CreditLimit = -100m };

        var result = await _sut.CreateCard(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        _cardService.Verify(s => s.CreateCardAsync(It.IsAny<CreateCardRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetCardBalance_ValidRequest_ReturnsOkWithBalance()
    {
        var cardId = Guid.NewGuid();
        _cardService.Setup(s => s.GetCardBalanceAsync(cardId, "Euro")).ReturnsAsync(850m);

        var result = await _sut.GetCardBalance(cardId, "Euro");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(850m, ok.Value);
    }

    [Fact]
    public async Task GetCardBalance_ServiceThrowsKeyNotFoundException_BubblesUp()
    {
        var cardId = Guid.NewGuid();
        _cardService.Setup(s => s.GetCardBalanceAsync(cardId, "Euro"))
                    .ThrowsAsync(new KeyNotFoundException("Card not found."));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetCardBalance(cardId, "Euro"));
    }
}
