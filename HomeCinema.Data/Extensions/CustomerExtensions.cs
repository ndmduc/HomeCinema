using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Data.Extensions
{
    public static class CustomerExtensions
    {
        public static bool UserExists(this IEntityBaseRepository<Customer> customerRepo, string email, string identityCard)
        {
            return customerRepo.GetAll().Any(c => c.Email == email || c.IdentifyCard.ToLower() == identityCard.ToLower());
        }

        public static string GetCustomerFullName(this IEntityBaseRepository<Customer> customerRepo, int customerId)
        {
            string customerName = string.Empty;
            var customer = customerRepo.GetSingle(customerId);
            return customer.FirstName + " " + customer.LastName;
        }
    }
}
