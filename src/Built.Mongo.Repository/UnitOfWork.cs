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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IClientSessionHandle session;
        private readonly Database db;

        public UnitOfWork(BuiltOptions config) : this(config.Url)
        {
        }

        public UnitOfWork(string connectionString) : this(new MongoUrl(connectionString))
        {
        }

        public UnitOfWork(MongoUrl url)
        {
            db = new Database(url);
            session = db.Client.StartSession();
            // 设置隔离级别;
            StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));
        }

        public IRepository<T> GetRepository<T>(string databaseName = "") where T : IEntity
        {
            return new Repository<T>(db.GetCollection<T>(databaseName))
            {
                Session = session
            };
        }

        public void StartTransaction(TransactionOptions transactionOptions = null)
        {
            session.StartTransaction(transactionOptions);
        }

        public void CommitTransaction(CancellationToken cancellationToken = default(CancellationToken))
        {
            Policy.Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) || i.InnerException.GetType() == typeof(SocketException))
                  .Or<MongoException>(i => i.HasErrorLabel("UnknownTransactionCommitResult"))
                  .Retry(3, (exception, retryCount) => { Console.WriteLine("Retry." + retryCount); })
                  .Execute(() =>
                     {
                         session.CommitTransaction(cancellationToken);
                     });
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Policy.Handle<MongoConnectionException>(i => i.InnerException.GetType() == typeof(IOException) || i.InnerException.GetType() == typeof(SocketException))
                   .Or<MongoException>(i => i.HasErrorLabel("UnknownTransactionCommitResult"))
                   .Retry(3, (exception, retryCount) => { Console.WriteLine("Retry." + retryCount); })
                   .ExecuteAsync(() =>
                   {
                       return session.CommitTransactionAsync(cancellationToken);
                   });
        }

        public void AbortTransaction(CancellationToken cancellationToken = default(CancellationToken))
        {
            session.AbortTransaction(cancellationToken);
        }

        public Task AbortTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return session.AbortTransactionAsync(cancellationToken);
        }
    }
}

/*

     // Start Transaction Intro Example 1
        public void UpdateEmployeeInfo(IMongoClient client, IClientSessionHandle session)
        {
            var employeesCollection = client.GetDatabase("hr").GetCollection<BsonDocument>("employees");
            var eventsCollection = client.GetDatabase("reporting").GetCollection<BsonDocument>("events");

            session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));

            try
            {
                employeesCollection.UpdateOne(
                    session,
                    Builders<BsonDocument>.Filter.Eq("employee", 3),
                    Builders<BsonDocument>.Update.Set("status", "Inactive"));
                eventsCollection.InsertOne(
                    session,
                    new BsonDocument
                    {
                        { "employee", 3 },
                        { "status", new BsonDocument { { "new", "Inactive" }, { "old", "Active" } } }
                    });
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Caught exception during transaction, aborting: {exception.Message}.");
                session.AbortTransaction();
                throw;
            }

            while (true)
            {
                try
                {
                    session.CommitTransaction(); // uses write concern set at transaction start
                    Console.WriteLine("Transaction committed.");
                    break;
                }
                catch (MongoException exception)
                {
                    // can retry commit
                    if (exception.HasErrorLabel("UnknownTransactionCommitResult"))
                    {
                        Console.WriteLine("UnknownTransactionCommitResult, retrying commit operation.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Error during commit.");
                        throw;
                    }
                }
            }
        }
     */

/*
 * var employeesCollection = client.GetDatabase("hr").GetCollection<BsonDocument>("employees");
            var eventsCollection = client.GetDatabase("reporting").GetCollection<BsonDocument>("events");

            session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority));
try
            {
                employeesCollection.UpdateOne(
                    session,
                    Builders<BsonDocument>.Filter.Eq("employee", 3),
                    Builders<BsonDocument>.Update.Set("status", "Inactive"));
                eventsCollection.InsertOne(
                    session,
                    new BsonDocument
                    {
                        { "employee", 3 },
                        { "status", new BsonDocument { { "new", "Inactive" }, { "old", "Active" } } }
                    });
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Caught exception during transaction, aborting: {exception.Message}.");
                session.AbortTransaction();
                throw;
            }
CommitWithRetry(session);

    public void CommitWithRetry(IClientSessionHandle session)
        {
            while (true)
            {
                try
                {
                    session.CommitTransaction();
                    Console.WriteLine("Transaction committed.");
                    break;
                }
                catch (MongoException exception)
                {
                    // can retry commit
                    if (exception.HasErrorLabel("UnknownTransactionCommitResult"))
                    {
                        Console.WriteLine("UnknownTransactionCommitResult, retrying commit operation");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"Error during commit: {exception.Message}.");
                        throw;
                    }
                }
            }
        }
*/