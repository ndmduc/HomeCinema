using HomeCinema.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System.Net.Http;
using HomeCinema.Models;
using System.Net;

namespace HomeCinema.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Customer> customerRepo;

        public CustomersController(IEntityBaseRepository<Customer> customerRepository,
            IEntityBaseRepository<Error> errorRepository, IUnitOfWork unitofwork) : base(errorRepository, unitofwork)
        {
            this.customerRepo = customerRepository;
        }

        [HttpGet]
        [Route("search/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Search(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Customer> customers = null;
                int totalMovies = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    customers = this.customerRepo.GetAll()
                                                        .OrderBy(c => c.ID)
                                                        .Where(c => c.LastName.ToLower().Contains(filter) || 
                                                                c.IdentifyCard.ToLower().Contains(filter) ||
                                                                c.FirstName.ToLower().Contains(filter)).ToList();

                }
                else
                {
                    customers = this.customerRepo.GetAll().ToList();
                }

                totalMovies = customers.Count();
                customers = customers.Skip(currentPage * currentPageSize).Take(currentPageSize).ToList();

                IEnumerable<CustomerViewModel> customersVm = AutoMapper.Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerViewModel>>(customers);

                PaginationSet<CustomerViewModel> pagedSet = new PaginationSet<CustomerViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                    Items = customersVm
                };

                response = request.CreateResponse<PaginationSet<CustomerViewModel>>(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }
    }
}