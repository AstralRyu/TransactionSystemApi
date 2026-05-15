using TransactionSystemApi.Models;

namespace TransactionSystemApi.Repositories
{
    public interface ICardRepository
    {
        Task<Card> AddCardAsync(Card card);
        Task<Card?> GetCardById(Guid cardId);
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

        public async Task<Card?> GetCardById(Guid cardId)
        {
            return await _dbContext.Cards.FindAsync(cardId);
        }
    }
    
}


