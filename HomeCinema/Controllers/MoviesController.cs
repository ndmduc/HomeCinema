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

        [AllowAnonymous]
        [Route("geted")]
        public HttpResponseMessage Geted(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Movie> movies = null;
                int totalMovies = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    movies = this.moviesRepo.GetAll().OrderBy(m => m.ID)
                                    .Where(m => m.Title.ToLower().Contains(filter.ToLower().Trim())).ToList();
                }
                else
                {
                    movies = this.moviesRepo.GetAll().ToList();
                }

                totalMovies = movies.Count();
                movies = movies.Skip(currentPage * currentPageSize).Take(currentPageSize).ToList();

                IEnumerable<MovieViewModel> moviesVM = Mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);
                PaginationSet<MovieViewModel> pagedSet = new PaginationSet<MovieViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMovies,
                    TotalPages = (int)Math.Ceiling((decimal)totalMovies/currentPageSize),
                    Items = moviesVM
                };

                response = request.CreateResponse<PaginationSet<MovieViewModel>>(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }

        [Route("details")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movie = this.moviesRepo.GetSingle(id);

                MovieViewModel movieVM = Mapper.Map<Movie, MovieViewModel>(movie);
                response = request.CreateResponse<MovieViewModel>(HttpStatusCode.OK, movieVM);
                return response;
            });
        }
    }
}