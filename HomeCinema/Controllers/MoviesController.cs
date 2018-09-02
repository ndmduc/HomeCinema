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
using System.IO;
using HomeCinema.Infrastructure.Extensions;

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

        [HttpGet]
        [Route("details")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var session = HttpContext.Current.Session;
                string text = "";
                if (session != null)
                {
                    if (session["username"] != null)
                        text = "SessionCheck: " + session["username"].ToString();
                    else
                        text = "session is null";
                }

                HttpResponseMessage response = null;
                var movie = this.moviesRepo.GetSingle(id);

                MovieViewModel movieVM = Mapper.Map<Movie, MovieViewModel>(movie);
                response = request.CreateResponse<MovieViewModel>(HttpStatusCode.OK, movieVM);
                return response;
            });
        }

        [MimeMultipart]
        [Route("images/upload")]
        public HttpResponseMessage Post(HttpRequestMessage request, int movieId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var movieOld = this.moviesRepo.GetSingle(movieId);
                if (movieOld==null)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid movie.");
                }
                else
                {
                    var uploadPath = HttpContext.Current.Server.MapPath("~/Content/images/movies");
                    var multipartFormDataSP = new UploadMultipartFormProvider(uploadPath);

                    // Read the MIME multipart asynchronously.
                    Request.Content.ReadAsMultipartAsync(multipartFormDataSP);
                    string localFileName = multipartFormDataSP.FileData.Select(m => m.LocalFileName).FirstOrDefault();

                    //Create response.
                    FileUploadResult fileUploadResult = new FileUploadResult
                    {
                        LocalFilePath = localFileName,
                        FileName = Path.GetFileName(localFileName),
                        FileLength = new FileInfo(localFileName).Length
                    };

                    // Update db
                    movieOld.Image = fileUploadResult.FileName;
                    this.moviesRepo.Edit(movieOld);
                    this.unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, fileUploadResult);
                }
                return response;
            });
        }

        [HttpPost]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, MovieViewModel movie)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var movieDb = this.moviesRepo.GetSingle(movie.ID);
                    if (movieDb==null)
                    {
                        response = request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid movie.");
                    }
                    else
                    {
                        movieDb.UpdateMovie(movie);
                        movie.Image = movieDb.Image;
                        this.moviesRepo.Edit(movieDb);

                        this.unitOfWork.Commit();
                        response = request.CreateResponse<MovieViewModel>(HttpStatusCode.OK, movie);
                    }
                }
                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, MovieViewModel movie)
        {
            return this.CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    Movie newMovie = new Movie();
                    newMovie.UpdateMovie(movie);

                    for (int i = 0; i < movie.NumberOfStocks; i++)
                    {
                        Stock stock = new Stock
                        {
                            IsAvailable = true,
                            Movie = newMovie,
                            UniqueKey = Guid.NewGuid()
                        };
                        newMovie.Stocks.Add(stock);

                        this.moviesRepo.Add(newMovie);
                        this.unitOfWork.Commit();

                        // Update view model
                        movie = Mapper.Map<Movie, MovieViewModel>(newMovie);
                        response = request.CreateResponse<MovieViewModel>(HttpStatusCode.Created, movie);
                    }
                }
                return response;
            });
        }
    }
}