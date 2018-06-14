using AutoMapper;
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
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/genres")]
    public class GenresController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Genre> genreRepo;

        public GenresController(IEntityBaseRepository<Genre> genresRepo, IEntityBaseRepository<Error> errorRepo,
            IUnitOfWork unitOfWork) : base(errorRepo, unitOfWork)
        {
            this.genreRepo = genresRepo;
        }

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var genres = this.genreRepo.GetAll().ToList();

                IEnumerable<GenreViewModel> genresVm = Mapper.Map<IEnumerable<Genre>, IEnumerable<GenreViewModel>>(genres);
                response = request.CreateResponse<IEnumerable<GenreViewModel>>(HttpStatusCode.OK, genresVm);
                return response;
            });
        }
    }
}