using HomeCinema.Entities;
using HomeCinema.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCinema.Services.Abstract
{
    public interface  IMembershipService
    {
        User CreateUser(string userName, string email, string password, int[] roles);

        User GetUser(int userid);

        List<Role> GetUserRoles(string username); 


        MembershipContext ValidateUser(string username, string password);
    }
}
