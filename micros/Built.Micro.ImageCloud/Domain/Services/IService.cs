using Built.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Built.Micro.ImageCloud.Domain.Services
{
    public interface IService<T> where T : IEntity
    {
        IRepository<T> Repository { get; }
    }
}