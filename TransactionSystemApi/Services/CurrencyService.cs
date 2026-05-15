using TransactionSystemApi.DTOs;
using TransactionSystemApi.Infrastructure;

namespace TransactionSystemApi.Services
{
    public interface ICurrencyService
    {
        Task<decimal> GetRateForDateAsync(string currency, DateOnly transactionDate);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly ITreasuryApiClient _client;
        
        public CurrencyService(ITreasuryApiClient client)
        {
            _client = client;
        }
        
        public async Task<decimal> GetRateForDateAsync(string currency, DateOnly transactionDate)
        {
            var result = await _client.GetRateByCurrencyDateAsync(currency, transactionDate);
            
            if (result is null || result.RecordDate < transactionDate.AddMonths(-6))
                throw new InvalidOperationException(
                    $"No exchange rate available for '{currency}' within 6 months of {transactionDate}.");

            return result.ExchangeRate;
        }
    }
}