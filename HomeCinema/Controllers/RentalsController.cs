using AutoMapper;
using HomeCinema.Data.Extensions;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Infrastructure.Core;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace HomeCinema.Controllers
{
    [Authorize(Roles="Admin")]
    [RoutePrefix("api/rentals")]
    public class RentalsController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Rental> rentalsRepo;
        private readonly IEntityBaseRepository<Customer> customersRepo;
        private readonly IEntityBaseRepository<Stock> stocksRepo;
        private readonly IEntityBaseRepository<Movie> moviesRepo;

        public RentalsController(IEntityBaseRepository<Rental> rentalsRepo, IEntityBaseRepository<Customer> customersRepo,
                IEntityBaseRepository<Stock> stocksRepo, IEntityBaseRepository<Movie> moviesRepo, 
                IEntityBaseRepository<Error> errorsRepo, IUnitOfWork unitofWork) : base(errorsRepo, unitofWork)
        {
            this.rentalsRepo = rentalsRepo;
            this.customersRepo = customersRepo;
            this.stocksRepo = stocksRepo;
            this.moviesRepo = moviesRepo;
        }

        [HttpGet]
        [Route("rentalhistory/{id:int}")]
        public HttpResponseMessage RentalHistory(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<RentalHistoryViewModel> rentalHistory = GetMovieRentalHistory(id);
                response = request.CreateResponse<List<RentalHistoryViewModel>>(HttpStatusCode.OK, rentalHistory);
                return response;
            });
        }

        [HttpPost]
        [Route("return/{rentalId:int}")]
        public HttpResponseMessage Return(HttpRequestMessage request, int rentalId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var rental = this.rentalsRepo.GetSingle(rentalId);
                if (rental == null)
                {
                    response = request.CreateResponse(HttpStatusCode.NotFound, "Invalid rental");
                }
                else
                {
                    rental.Status = "Returned";
                    rental.Stock.IsAvailable = true;
                    rental.ReturnedDate = DateTime.Now;
                    this.unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("rent/{customerId:int}/{stockId:int}")]
        public HttpResponseMessage Rent(HttpRequestMessage request, int customerId, int stockId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var customer = this.customersRepo.GetSingle(customerId);
                var stock = this.stocksRepo.GetSingle(stockId);
                if (customer==null || stock==null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid Customer or Stock");
                }
                else
                {
                    if (stock.IsAvailable)
                    {
                        Rental rental = new Rental
                        {
                            CustomerId = customerId,
                            StockId = stockId,
                            RentalDate = DateTime.Now,
                            Status = "Borrowed"
                        };

                        this.rentalsRepo.Add(rental);

                        stock.IsAvailable = false;
                        this.unitOfWork.Commit();
                        RentalViewModel rentalVm = Mapper.Map<Rental, RentalViewModel>(rental);
                        response = request.CreateResponse(HttpStatusCode.Created, rentalVm);
                    }
                    else
                    {
                        response = request.CreateErrorResponse(HttpStatusCode.BadRequest, "Selected stock is not available anymore");

                    }
                }
                return response;
            });
        }

        private List<RentalHistoryViewModel> GetMovieRentalHistory(int movieId)
        {
            List<RentalHistoryViewModel> rentalHistory = new List<RentalHistoryViewModel>();
            List<Rental> rentals = new List<Rental>();
            var movie = this.moviesRepo.GetSingle(movieId);

            foreach (var stock in movie.Stocks)
            {
                rentals.AddRange(stock.Rentals);
            }

            foreach (var rental in rentals)
            {
                RentalHistoryViewModel historyItem = new RentalHistoryViewModel()
                {
                    ID = rental.ID,
                    StockId = rental.StockId,
                    RentalDate = rental.RentalDate,
                    ReturnedDate = rental.ReturnedDate.HasValue ? rental.ReturnedDate : null,
                    Status = rental.Status,
                    Customer = this.customersRepo.GetCustomerFullName(rental.CustomerId)
                };

                rentalHistory.Add(historyItem);
            }

            rentalHistory.Sort((r1, r2) => r2.RentalDate.CompareTo(r1.RentalDate));
            return rentalHistory;
        }
    }
}