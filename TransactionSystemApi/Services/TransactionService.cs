using TransactionSystemApi.Repositories;

namespace TransactionSystemApi.Services
{
    public interface ITransactionService
    {
        
    }
    
    
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
    }
}