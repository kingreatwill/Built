using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Built.Mongo
{
    /// <summary>
    /// https://docs.mongodb.com/master/core/transactions/#transactions-and-replica-sets
    ///  Start a mongod —replSet <replsetname> then do rs.initiate() in the mongo shell
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private IClientSessionHandle session;
        private readonly Database db;

        private readonly bool DotUseTransaction;

        public UnitOfWork(BuiltOptions config)
        {
            DotUseTransaction = config.DotUseTransaction;
            db = new Database(config.Url);
            if (DotUseTransaction) return;
            // 设置隔离级别;
            StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));
        }

        public UnitOfWork(string connectionString) : this(new MongoUrl(connectionString))
        {
        }

        public UnitOfWork(MongoUrl url)
        {
            db = new Database(url);
            StartTransaction();
        }

        public IRepository<T> GetRepository<T>(string databaseName = "") where T : IEntity
        {
            return new Repository<T>(db.GetCollection<T>(databaseName))
            {
                Session = DotUseTransaction ? null : session
            };
        }

        public void StartTransaction(TransactionOptions transactionOptions = null)
        {
            if (DotUseTransaction) return;
            if (session == null)
                session = db.Client.StartSession();
            session.StartTransaction(transactionOptions);
        }

        public void CommitTransaction(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DotUseTransaction) return;
            Policy.Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) || i.InnerException.GetType() == typeof(SocketException))
                  .Or<MongoException>(i => i.HasErrorLabel("UnknownTransactionCommitResult") || i.HasErrorLabel("TransientTransactionError"))//TransientTransactionError
                  .Retry(3, (exception, retryCount) => { Console.WriteLine("Retry." + retryCount); })
                  .Execute(() =>
                     {
                         session.CommitTransaction(cancellationToken);
                     });
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DotUseTransaction) return Task.FromResult(0);
            return Policy.Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) || i.InnerException.GetType() == typeof(SocketException))
                   .Or<MongoException>(i => i.HasErrorLabel("UnknownTransactionCommitResult") || i.HasErrorLabel("TransientTransactionError"))
                   .Retry(3, (exception, retryCount) => { Console.WriteLine("Retry." + retryCount); })
                   .ExecuteAsync(() =>
                   {
                       return session.CommitTransactionAsync(cancellationToken);
                   });
        }

        public void AbortTransaction(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DotUseTransaction) return;
            session.AbortTransaction(cancellationToken);
        }

        public Task AbortTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DotUseTransaction) return Task.FromResult(0);
            return session.AbortTransactionAsync(cancellationToken);
        }
    }
}