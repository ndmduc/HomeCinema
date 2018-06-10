using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Infrastructure.Core
{
    public class ApiControllerBase : ApiController
    {
        protected readonly IEntityBaseRepository<Error> errorRepo;

        protected readonly IUnitOfWork unitOfWork;

        public ApiControllerBase(IEntityBaseRepository<Error> errorRepository, IUnitOfWork unitofwork)
        {
            this.errorRepo = errorRepository;
            this.unitOfWork = unitofwork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;
            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch(Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex.InnerException.Message);
            }

            return response;
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

                this.errorRepo.Add(error);
                this.unitOfWork.Commit();
            }
            catch { }
            
        }
    }
}