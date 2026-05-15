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
        public async Task<decimal> GetRateForDateAsync(string currency, DateOnly transactionDate)
        {
            var result = await _client.GetRateOnOrBeforeDateAsync(currency, transactionDate);
            
            if (result is null)
                throw new InvalidOperationException(
                    $"No exchange rate available for '{currency}' within 6 months of {transactionDate}.");

            return 0;
        }
    }
}