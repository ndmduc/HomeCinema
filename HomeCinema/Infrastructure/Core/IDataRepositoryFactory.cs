using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System.Net.Http;

namespace HomeCinema.Infrastructure.Core
{
    public interface IDataRepositoryFactory
    {
        IEntityBaseRepository<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntityBase, new();
    }
}