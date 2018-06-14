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
using System.Collections;
using HomeCinema.Models;
using AutoMapper;
using System.Net;

namespace HomeCinema.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/movies")]
    public class MoviesController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Movie> moviesRepo;

        private readonly IEntityBaseRepository<Rental> rentalsRepo;

        private readonly IEntityBaseRepository<Stock> stocksRepo;

        private readonly IEntityBaseRepository<Customer> customersRepo;

        public MoviesController(IEntityBaseRepository<Movie> moviesRepo, IEntityBaseRepository<Rental> rentalsRepo,
            IEntityBaseRepository<Stock> stocksRepo, IEntityBaseRepository<Customer> customersRepo,
            IEntityBaseRepository<Error> errorRepository, IUnitOfWork unitofwork
            ) : base(errorRepository, unitofwork)
        {
            this.moviesRepo = moviesRepo;
            this.rentalsRepo = rentalsRepo;
            this.stocksRepo = stocksRepo;
            this.customersRepo = customersRepo;
        }

        [AllowAnonymous]
        [Route("latest")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movies = this.moviesRepo.GetAll().OrderByDescending(m => m.ReleaseDate).Take(6).ToList();

                IEnumerable<MovieViewModel> moviesVm = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);
                response = request.CreateResponse<IEnumerable<MovieViewModel>>(HttpStatusCode.OK, moviesVm);
                return response;
            });
        }
    }
}