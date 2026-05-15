using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;

namespace TransactionSystemApi.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request);

        Task<ConvertedTransaction> GetTransactionAsync(Guid transactionId, string currency);
    }


    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrencyService _currencyService;

        public TransactionService(ITransactionRepository transactionRepository, ICurrencyService currencyService)
        {
            _transactionRepository = transactionRepository;
            _currencyService = currencyService;
        }

        public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Description = request.Description,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CardId = request.CardId
            };
            return await _transactionRepository.AddTransactionAsync(transaction);
        }

        public async Task<ConvertedTransaction> GetTransactionAsync(Guid transactionId, string currency)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            var exchangeRate = await _currencyService.GetRateForDateAsync(currency, transaction.Date);

            var convertedAmount = Math.Round(transaction.Amount * exchangeRate, 2);

            return new ConvertedTransaction()
            {
                Id = transaction.Id,
                CardId = transaction.CardId,
                Description = transaction.Description,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Currency = currency,
                ExchangeRate = exchangeRate,
                ConvertedAmount = convertedAmount
            };
        }
    }
}