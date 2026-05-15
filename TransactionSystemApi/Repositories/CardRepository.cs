using TransactionSystemApi.Models;

namespace TransactionSystemApi.Repositories
{
    public interface ICardRepository
    {
        Task<Card> AddCardAsync(Card card);
        Task<Card?> GetByIdWithTransactionsAsync(Guid id);
    }
    
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _dbContext;
        
        public CardRepository(AppDbContext db)
        {
            _dbContext = db;
        }

        public async Task<Card> AddCardAsync(Card card)
        {
            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();
            return card;
        }

        public async Task<Card> GetByIdWithTransactionsAsync(Guid id)
        {
            return null;
        }

    }
    
}


