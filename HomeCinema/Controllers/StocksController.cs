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
using System.Web.Http;

namespace HomeCinema.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/stocks")]
    public class StocksController : ApiControllerBase
    {
        private readonly IEntityBaseRepository<Stock> stockRepo;

        public StocksController(IEntityBaseRepository<Stock> stockRepos, IEntityBaseRepository<Error> errorRepos,
            IUnitOfWork unitofWork) : base(errorRepos, unitofWork)
        {
            this.stockRepo = stockRepos;
        }

        [Route("movie/{id:int}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            IEnumerable<Stock> stocks = null;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                stocks = this.stockRepo.GetAvailableItems(id);
                IEnumerable<StockViewModel> stocksVM = Mapper.Map<IEnumerable<Stock>, IEnumerable<StockViewModel>>(stocks);
                response = request.CreateResponse<IEnumerable<StockViewModel>>(HttpStatusCode.OK, stocksVM);
                return response;
            });
        }
    }
}