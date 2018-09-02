using HomeCinema.Entities;
using HomeCinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeCinema.Infrastructure.Extensions
{
    public static class EntitiesExtensions
    {
        public static void UpdateCustomer(this Customer customer, CustomerViewModel customerVm)
        {
            customer.FirstName = customerVm.FirstName;
            customer.LastName = customerVm.LastName;
            customer.IdentifyCard = customerVm.IdentifyCard;
            customer.Mobile = customerVm.Mobile;
            customer.DateOfBirth = customerVm.DateOfBirth;
            customer.Email = customerVm.Email;
            customer.UniqueKey = (customerVm.Uniquekey == null || customerVm.Uniquekey == Guid.Empty) ?
                Guid.NewGuid() : customerVm.Uniquekey;
            customer.RegistrationDate = (customer.RegistrationDate == DateTime.MinValue
                                                ? DateTime.Now : customerVm.RegistrationDate);
        }

        public static void UpdateMovie(this Movie movie, MovieViewModel movieVm)
        {
            movie.Title = movieVm.Title;
            movie.Description = movieVm.Description;
            movie.GenreId = movieVm.GenreId;
            movie.Director = movieVm.Director;
            movie.Writer = movieVm.Writer;
            movie.Producer = movieVm.Producer;
            movie.Rating = movieVm.Rating;
            movie.TrailerURI = movieVm.TrailerURI;
            movie.ReleaseDate = movieVm.ReleaseDate;
        }
    }
}