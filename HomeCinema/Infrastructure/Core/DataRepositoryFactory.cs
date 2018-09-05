using HomeCinema.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeCinema.Data.Repositories;
using System.Net.Http;
using HomeCinema.Infrastructure.Extensions;

namespace HomeCinema.Infrastructure.Core
{
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        IEntityBaseRepository<T> IDataRepositoryFactory.GetDataRepository<T>(HttpRequestMessage request)
        {
            return request.GetDataRepository<T>();
        }
    }
}