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
using AutoMapper;
using HomeCinema.Infrastructure.Extensions;
using HomeCinema.Data.Extensions;

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

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, CustomerViewModel customer){
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                                                                                .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Customer _customer = this.customerRepo.GetSingle(customer.ID);
                    _customer.UpdateCustomer(customer);

                    this.unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("register")]
        public HttpResponseMessage Register(HttpRequestMessage request, CustomerViewModel customer)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                        .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    if(this.customerRepo.UserExists(customer.Email, customer.IdentifyCard))
                    {
                        ModelState.AddModelError("Invalid user", "Email or Identify Card number already exists.");
                        response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());

                    }
                    else
                    {
                        Customer newCustomer = new Customer();
                        newCustomer.UpdateCustomer(customer);
                        this.customerRepo.Add(newCustomer);

                        this.unitOfWork.Commit();

                        // Update view model
                        customer = Mapper.Map<Customer, CustomerViewModel>(newCustomer);
                        response = request.CreateResponse(HttpStatusCode.Created, customer);
                    }
                }
                return response;
            });
        }
    }
}