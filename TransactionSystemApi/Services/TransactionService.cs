using TransactionSystemApi.DTOs;
using TransactionSystemApi.Infrastructure;
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
        private readonly ITreasuryApiClient _client;

        public TransactionService(ITransactionRepository transactionRepository, ITreasuryApiClient client)
        {
            _transactionRepository = transactionRepository;
            _client = client;
        }

        public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date,
                CardId = request.CardId
            };
            return await _transactionRepository.AddTransactionAsync(transaction);
        }

        public async Task<ConvertedTransaction> GetTransactionAsync(Guid transactionId, string currency)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with id {transactionId} does not exist");
            }
            var exchangeRate = await _client.GetRateForDateByCurrencyAsync(currency, transaction.Date);

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