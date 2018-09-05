using AutoMapper;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HomeCinema.Controllers
{
    [Authorize(Roles ="Admin")]
    [RoutePrefix("api/rentalsextended")]
    public class RentalsExtendedController : ApiControllerBaseExtended
    {
        public RentalsExtendedController(IDataRepositoryFactory dataRepos, IUnitOfWork unitofwork) 
            : base(dataRepos, unitofwork)
        {

        }

        [HttpPost]
        [Route("rent/{customerId:int}/{stockId:int}")]
        public HttpResponseMessage Rent(HttpRequestMessage request, int customerId, int stockId)
        {
            this.requiredRepo = new List<Type>() { typeof(Customer), typeof(Stock), typeof(Rental) };
            return this.CreateHttpResponse(request, this.requiredRepo, () =>
            {
                HttpResponseMessage response = null;
                var customer = this.customersRepo.GetSingle(customerId);
                var stock = this.stocksRepo.GetSingle(stockId);

                if (customer == null || stock==null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid customer of stock");
                }
                else
                {
                    if (stock.IsAvailable)
                    {
                        Rental _rental = new Rental()
                        {
                            CustomerId = customerId,
                            StockId = stockId,
                            RentalDate = DateTime.Now,
                            Status = "Borrowed"
                        };
                        this.rentalsRepo.Add(_rental);
                        stock.IsAvailable = false;
                        this.unitOfWork.Commit();
                        RentalViewModel rentalVm = Mapper.Map<Rental, RentalViewModel>(_rental);
                        response = request.CreateResponse<RentalViewModel>(HttpStatusCode.Created, rentalVm);
                    }
                    else
                        response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Selected stock is not available anymore");

            }
                return response;
            });
        }
    }
}