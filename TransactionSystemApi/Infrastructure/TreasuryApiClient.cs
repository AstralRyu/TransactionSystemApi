using TransactionSystemApi.DTOs.TreasureApi;

namespace TransactionSystemApi.Infrastructure
{
    public interface ITreasuryApiClient
    {
        Task<TreasuryApiResponse> GetRateOnOrBeforeDateAsync(string currency, DateOnly transactionDate);
    }
    
    public class TreasuryApiClient : ITreasuryApiClient
    {
        private readonly HttpClient _httpClient;
        
        public async Task<TreasuryApiResponse> GetRateOnOrBeforeDateAsync(string currency, DateOnly transactionDate)
        {
            var url = "";
            var response = await _httpClient.GetFromJsonAsync<TreasuryApiResponse>(url);
            return response;
        }
    }
    
 
}

