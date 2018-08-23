using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Built.Mongo
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>(string databaseName = "") where T : IEntity;

        void StartTransaction(TransactionOptions transactionOptions = null);

        void CommitTransaction(CancellationToken cancellationToken = default(CancellationToken));

        Task CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));

        void AbortTransaction(CancellationToken cancellationToken = default(CancellationToken));

        Task AbortTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}