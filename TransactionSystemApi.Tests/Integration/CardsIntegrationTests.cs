using System.Net;
using System.Net.Http.Json;
using Moq;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;

namespace TransactionSystemApi.Tests.Integration;

public class CardsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CardsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_card_create_ValidRequest_Returns200WithCard()
    {
        var request = new CreateCardRequest { CreditLimit = 2000m };

        var response = await _client.PostAsJsonAsync("/card/create", request);
        var card = await response.Content.ReadFromJsonAsync<Card>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(card);
        Assert.Equal(2000m, card.CreditLimit);
        Assert.NotEqual(Guid.Empty, card.Id);
    }

    [Fact]
    public async Task POST_card_create_ZeroCreditLimit_Returns400()
    {
        var request = new CreateCardRequest { CreditLimit = 0 };

        var response = await _client.PostAsJsonAsync("/card/create", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task POST_card_create_NegativeCreditLimit_Returns400()
    {
        var request = new CreateCardRequest { CreditLimit = -1m };

        var response = await _client.PostAsJsonAsync("/card/create", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GET_card_balance_CardNotFound_Returns404()
    {
        var response = await _client.GetAsync($"/card/{Guid.NewGuid()}/balance?currency=Euro");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_card_balance_ValidCard_Returns200WithBalance()
    {
        // Create a card first
        var createResponse = await _client.PostAsJsonAsync("/card/create", new CreateCardRequest { CreditLimit = 500m });
        var card = await createResponse.Content.ReadFromJsonAsync<Card>();

        _factory.TreasuryApiClient
            .Setup(c => c.GetLatestRateByCurrencyAsync("Euro"))
            .ReturnsAsync(1.0m);

        var response = await _client.GetAsync($"/card/{card!.Id}/balance?currency=Euro");
        var balance = await response.Content.ReadFromJsonAsync<decimal>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(500m, balance);
    }
}
