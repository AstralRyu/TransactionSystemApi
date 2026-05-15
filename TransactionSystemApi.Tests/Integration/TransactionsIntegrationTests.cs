using System.Net;
using System.Net.Http.Json;
using Moq;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;

namespace TransactionSystemApi.Tests.Integration;

public class TransactionsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TransactionsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<Card> CreateCardAsync(decimal creditLimit = 1000m)
    {
        var response = await _client.PostAsJsonAsync("/card/create", new CreateCardRequest { CreditLimit = creditLimit });
        return (await response.Content.ReadFromJsonAsync<Card>())!;
    }

    [Fact]
    public async Task POST_transaction_create_ValidRequest_Returns200WithTransaction()
    {
        var card = await CreateCardAsync();
        var date = new DateOnly(2025, 6, 1);
        var request = new CreateTransactionRequest
        {
            Amount = 150m,
            Description = "Supermarket",
            Date = date,
            CardId = card.Id
        };

        var response = await _client.PostAsJsonAsync("/transaction/create", request);
        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(transaction);
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(150m, transaction.Amount);
        Assert.Equal("Supermarket", transaction.Description);
        Assert.Equal(card.Id, transaction.CardId);
    }

    [Fact]
    public async Task GET_transaction_get_TransactionNotFound_Returns404()
    {
        var response = await _client.GetAsync($"/transaction/{Guid.NewGuid()}/get?currency=Euro");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_transaction_get_ValidTransaction_Returns200WithConvertedAmount()
    {
        var card = await CreateCardAsync();
        var date = new DateOnly(2025, 4, 10);
        var createRequest = new CreateTransactionRequest
        {
            Amount = 200m,
            Description = "Hotel",
            Date = date,
            CardId = card.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/transaction/create", createRequest);
        var transaction = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        _factory.TreasuryApiClient
            .Setup(c => c.GetRateForDateByCurrencyAsync("Euro", date))
            .ReturnsAsync(1.5m);

        var response = await _client.GetAsync($"/transaction/{transaction!.Id}/get?currency=Euro");
        var converted = await response.Content.ReadFromJsonAsync<ConvertedTransaction>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(converted);
        Assert.Equal(200m, converted.Amount);
        Assert.Equal(1.5m, converted.ExchangeRate);
        Assert.Equal(300m, converted.ConvertedAmount);
        Assert.Equal("Euro", converted.Currency);
    }

    [Fact]
    public async Task GET_transaction_get_NoExchangeRateAvailable_Returns400()
    {
        var card = await CreateCardAsync();
        var date = new DateOnly(2025, 1, 1);
        var createRequest = new CreateTransactionRequest
        {
            Amount = 50m,
            Description = "Taxi",
            Date = date,
            CardId = card.Id
        };
        var createResponse = await _client.PostAsJsonAsync("/transaction/create", createRequest);
        var transaction = await createResponse.Content.ReadFromJsonAsync<Transaction>();

        _factory.TreasuryApiClient
            .Setup(c => c.GetRateForDateByCurrencyAsync("UnknownCurrency", date))
            .ThrowsAsync(new InvalidOperationException("No exchange rate available."));

        var response = await _client.GetAsync($"/transaction/{transaction!.Id}/get?currency=UnknownCurrency");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
