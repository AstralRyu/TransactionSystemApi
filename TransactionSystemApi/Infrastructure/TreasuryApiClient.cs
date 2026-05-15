using Microsoft.Extensions.Options;
using TransactionSystemApi.DTOs.TreasureApi;

namespace TransactionSystemApi.Infrastructure
{
    public interface ITreasuryApiClient
    {
        Task<decimal> GetRateForDateByCurrencyAsync(string currency, DateOnly transactionDate);
        
        Task<decimal> GetLatestRateByCurrencyAsync(string currency);
    }
    
    public class TreasuryApiClient : ITreasuryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string? baseUrl;

        public TreasuryApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            baseUrl = configuration.GetValue<string>("TreasuryApiUrl");
        }
        
        public async Task<decimal> GetRateForDateByCurrencyAsync(string currency, DateOnly transactionDate)
        {
            if (baseUrl == null)
            {  
                throw new NullReferenceException("TreasuryApiUrl is missing");
            }
            
            var builder = new UriBuilder(baseUrl);
            builder.Query = $"fields=country_currency_desc,exchange_rate,record_date" +
                            $"&filter=country_currency_desc:eq:{currency},record_date:lte:{transactionDate:yyyy-MM-dd}" +
                            $"&sort=-record_date" +
                            "&page[number]=1&page[size]=1";
            var url = builder.Uri;
            
            var response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url);

            var exchangeRate = response?.Data?.FirstOrDefault();
            
            if (exchangeRate is null || exchangeRate.RecordDate < transactionDate.AddMonths(-6))
                throw new InvalidOperationException(
                    $"No exchange rate available for '{currency}' within 6 months of {transactionDate}.");

            return exchangeRate.ExchangeRate;
        }

        public async Task<decimal> GetLatestRateByCurrencyAsync(string currency)
        {
            var builder = new UriBuilder(baseUrl);
            builder.Query = $"fields=country_currency_desc,exchange_rate,record_date" +
                            $"&filter=country_currency_desc:eq:{currency}" +
                            $"&sort=-record_date" +
                            "&page[number]=1&page[size]=1";
            var url = builder.Uri;
            
            var response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url);
            var exchangeRate = response?.Data?.FirstOrDefault();
            
            if (exchangeRate is null)
                throw new InvalidOperationException(
                    $"No exchange rate available for '{currency}'.");

            return exchangeRate.ExchangeRate;
        }
    }
}
