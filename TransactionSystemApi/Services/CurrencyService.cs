using TransactionSystemApi.DTOs;

namespace TransactionSystemApi.Services
{
    public interface ICurrencyService
    {
        Task<GetCardBalanceResponse> GetCardBalanceAsync(string currency);
    }

    public class CurrencyService : ICurrencyService
    {
        public async Task<GetCardBalanceResponse> GetCardBalanceAsync(string currency)
        {
            return null;
        }
    }
}