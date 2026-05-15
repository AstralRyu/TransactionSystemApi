namespace TransactionSystemApi.Repositories
{
    public interface ITransactionRepository
    {
        
    }
    
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _dbContext;

        public TransactionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
    }
}

