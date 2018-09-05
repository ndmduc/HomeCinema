using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Infrastructure.Core
{
    public class ApiControllerBaseExtended : ApiController
    {
        protected List<Type> requiredRepo;

        protected readonly IDataRepositoryFactory dataRepoFactory;

        protected IEntityBaseRepository<Error> errorsRepo;

        protected IEntityBaseRepository<Movie> moviesRepo;

        protected IEntityBaseRepository<Rental> rentalsRepo;

        protected IEntityBaseRepository<Stock> stocksRepo;

        protected IEntityBaseRepository<Customer> customersRepo;

        protected IUnitOfWork unitOfWork;

        private HttpRequestMessage requestMessage;

        public ApiControllerBaseExtended(IDataRepositoryFactory dataRepo, IUnitOfWork unitofwork)
        {
            this.dataRepoFactory = dataRepo;
            this.unitOfWork = unitofwork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, List<Type> repos, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                this.requestMessage = request;
                InitRepositories(repos);
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void InitRepositories(List<Type> entities)
        {
            this.errorsRepo = this.dataRepoFactory.GetDataRepository<Error>(this.requestMessage);

            if (entities.Any(e => e.FullName == typeof(Movie).FullName))
            {
                this.moviesRepo = this.dataRepoFactory.GetDataRepository<Movie>(this.requestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Rental).FullName))
            {
                this.rentalsRepo = this.dataRepoFactory.GetDataRepository<Rental>(this.requestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Customer).FullName))
            {
                this.customersRepo = this.dataRepoFactory.GetDataRepository<Customer>(this.requestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Stock).FullName))
            {
                this.stocksRepo = this.dataRepoFactory.GetDataRepository<Stock>(this.requestMessage);
            }

            //if (entities.Any(e => e.FullName == typeof(User).FullName))
            //{
            //    this.stocksRepo = this.dataRepoFactory.GetDataRepository<User>(this.requestMessage);
            //}
        }

        private void LogError(Exception ex)
        {
            try
            {
                Error error = new Error
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    DateCreated = DateTime.Now
                };

                this.errorsRepo.Add(error);
                this.unitOfWork.Commit();
            }
            catch { }

        }
    }
}