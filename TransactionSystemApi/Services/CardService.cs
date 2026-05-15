using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;
using System.Linq;
using TransactionSystemApi.Infrastructure;

namespace TransactionSystemApi.Services
{
    public interface ICardService
    {
        Task<Card> CreateCardAsync(CreateCardRequest request);
        
        Task<decimal> GetCardBalanceAsync(Guid id, string currency);
    }
    
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITreasuryApiClient _client;

        public CardService(ICardRepository cardRepository, ITransactionRepository transactionRepository, ITreasuryApiClient client)
        {
            _cardRepository = cardRepository;
            _transactionRepository = transactionRepository;
            _client = client;
        }
        
        public async Task<Card> CreateCardAsync(CreateCardRequest request)
        {
            if (request.CreditLimit <= 0)
                throw new ArgumentException("Credit limit must be greater than zero.");

            var card = new Card
            {
                Id = Guid.NewGuid(),
                CreditLimit = request.CreditLimit
            };
            
            return await _cardRepository.AddCardAsync(card); 
        }

        public async Task<decimal> GetCardBalanceAsync(Guid cardId, string currency)
        {
            var card = await _cardRepository.GetCardById(cardId);
            if (card == null)
            {
                throw new KeyNotFoundException($"Card with id {cardId} not found.");
            }
            var cardTransactions = await _transactionRepository.GetTransactionsByCardIdAsync(cardId);
            
            var totalAmount = cardTransactions.Sum(t => t.Amount);
            var balance = card.CreditLimit - totalAmount;
            
            var exchangeRate = await _client.GetLatestRateByCurrencyAsync(currency);
            balance = Math.Round(balance * exchangeRate, 2);
            
            return balance;
        }
    }
}



