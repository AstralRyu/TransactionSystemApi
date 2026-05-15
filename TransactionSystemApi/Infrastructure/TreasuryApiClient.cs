using Microsoft.Extensions.Options;
using TransactionSystemApi.DTOs.TreasureApi;

namespace TransactionSystemApi.Infrastructure
{
    public interface ITreasuryApiClient
    {
        Task<ExchangeRateResult?> GetRateByCurrencyDateAsync(string currency, DateOnly transactionDate);
    }
    
    public class TreasuryApiClient : ITreasuryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TreasuryApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public async Task<ExchangeRateResult?> GetRateByCurrencyDateAsync(string currency, DateOnly transactionDate)
        {
            var baseUrl = _configuration.GetValue<string>("TreasuryApiUrl");

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
            return exchangeRate;
        }
    }
}
