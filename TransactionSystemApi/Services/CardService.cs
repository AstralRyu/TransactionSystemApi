using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Repositories;

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

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
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

        public async Task<decimal> GetCardBalanceAsync(Guid id, string currency)
        {
            return 0;
        }
    }
}

    
