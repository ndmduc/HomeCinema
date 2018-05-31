using HomeCinema.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeCinema.Entities;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Extensions;
using HomeCinema.Services.Utilities;
using System.Security.Principal;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        #region Variables
        private readonly IEntityBaseRepository<User> userRepo;

        private readonly IEntityBaseRepository<Role> roleRepo;

        private readonly IEntityBaseRepository<UserRole> userroleRepo;

        private readonly IEncryptionService encryptionService;

        private readonly IUnitOfWork unitOfWork;

        #endregion

        public MembershipService(IEntityBaseRepository<User> userRepo, IEntityBaseRepository<Role> roleRepo, 
            IEntityBaseRepository<UserRole> userroleRepo, IEncryptionService encryptionService, IUnitOfWork unitofWork)
        {
            this.userRepo = userRepo;
            this.roleRepo = roleRepo;
            this.userroleRepo = userroleRepo;
            this.encryptionService = encryptionService;
            this.unitOfWork = unitofWork;
        }

        #region IMembershipService implement
        public User CreateUser(string userName, string email, string password, int[] roles)
        {
            var existingUser = this.userRepo.GetSingleByUsername(userName);
            if (existingUser != null)
            {
                throw new Exception("Username is already in use");
            }

            var passwordSalt = this.encryptionService.CreateSalt();
            var user = new User()
            {
                UserName = userName,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = this.encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };

            this.userRepo.Add(user);
            this.unitOfWork.Commit();

            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    AddUser2Role(user, role);
                }
            }

            this.unitOfWork.Commit();
            return user;
        }

        public List<Role> GetUserRoles(string username)
        {
            List<Role> result = new List<Role>();
            var existingUser = this.userRepo.GetSingleByUsername(username);
            if (existingUser != null)
            {
                foreach(var userrole in existingUser.UserRoles)
                {
                    result.Add(userrole.Role);
                }
            }

            return result.Distinct().ToList();
        }

        public User GetUser(int userid)
        {
            return this.userRepo.GetSingle(userid);
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();
            var user = this.userRepo.GetSingleByUsername(username);
            if (user != null && IsUserValid(user, password))
            {
                var userRole = this.GetUserRoles(username);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.UserName);
                membershipCtx.Principal = new GenericPrincipal(identity, userRole.Select(e => e.Name).ToArray());
            }

            return membershipCtx;
        }
        #endregion

        #region Helper methods
        private void AddUser2Role(User user, int roleId)
        {
            var role = this.roleRepo.GetSingle(roleId);
            if (role == null)
            {
                throw new ApplicationException("Role doesn't exist");
            }

            var userRole = new UserRole() { RoleId = role.ID, UserId = user.ID };
            this.userroleRepo.Add(userRole);
        }

        private bool IsPasswordValid(User user, string password)
        {
            return string.Equals(this.encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        private bool IsUserValid(User user, string password)
        {
            if (IsPasswordValid(user, password))
            {
                return !user.IsLocked;
            }

            return false;
        }
        #endregion
    }
}
