namespace Built.Mongo
{
    public interface IService<T> where T : IEntity
    {
        IRepository<T> Repository { get; }
    }
}