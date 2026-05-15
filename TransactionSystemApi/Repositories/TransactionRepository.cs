using TransactionSystemApi.Models;

namespace TransactionSystemApi.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId);
    }
    
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _dbContext;

        public TransactionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Transaction> AddTransactionAsync(Transaction transaction)
        {
            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        {
            return await _dbContext.Transactions.FindAsync(transactionId);
        }
    }
}

