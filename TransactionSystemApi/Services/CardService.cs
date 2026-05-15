using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;

namespace TransactionSystemApi.Services
{
    public interface ICardService
    {
        Task<Card> CreateCardAsync(decimal creditLimit);
        
        Task<GetCardBalanceResponse> GetCardBalanceAsync(string currency);
    }
    
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        
        public async Task<Card> CreateCardAsync(decimal creditLimit)
        {
            if (creditLimit <= 0)
                throw new ArgumentException("Credit limit must be greater than zero.");

            var card = new Card
            {
                Id = Guid.NewGuid(),
                CreditLimit = creditLimit
            };
            
            return await _cardRepository.AddCardAsync(card); 
        }

        public async Task<GetCardBalanceResponse> GetCardBalanceAsync(string currency)
        {
            return null;
        }
    }
}

    
