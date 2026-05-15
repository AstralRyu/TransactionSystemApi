using System.Text.Json.Serialization;

namespace TransactionSystemApi.DTOs.TreasureApi;

public record ExchangeRateResult(
    [property: JsonPropertyName("country_currency_desc")] string Currency,
    [property: JsonPropertyName("exchange_rate")] decimal ExchangeRate,
    [property: JsonPropertyName("record_date")] DateOnly RecordDate
);

public class TreasuryApiResponse
{
    [JsonPropertyName("data")]
    public List<ExchangeRateResult>? Data { get; set; }
}