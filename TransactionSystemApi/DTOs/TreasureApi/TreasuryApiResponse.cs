using System.Text.Json.Serialization;

namespace TransactionSystemApi.DTOs.TreasureApi;

public record ExchangeRateResult(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("exchange_rate")] decimal ExchangeRate,
    [property: JsonPropertyName("record_date")] DateOnly RecordDate
);

public class TreasuryApiResponse
{
    [JsonPropertyName("data")]
    public List<ExchangeRateResult>? Data { get; set; }
}