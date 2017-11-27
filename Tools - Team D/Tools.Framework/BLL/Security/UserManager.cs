using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Providers.Entities;
using Tools.Framework.DAL;
using Tools.Framework.DAL.Security;
using Tools.Framework.Entities.Security;

namespace Tools.Framework.BLL.Security
{
    [DataObject]
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager()
            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
        {
        }

        #region Constants
        private const string STR_DEFAULT_PASSWORD = "Pa$$word1";
        /// <summary>Requires FirstName and LastName</summary>
        private const string STR_USERNAME_FORMAT = "{0}.{1}";
        /// <summary>Requires UserName</summary>
        private const string STR_EMAIL_FORMAT = "{0}@ToolsRUs.tba";
        private const string STR_WEBMASTER_USERNAME = "Webmaster";
        #endregion

        public void AddWebMaster()
        {
            // Add a web  master user
            if (!Users.Any(u => u.UserName.Equals(STR_WEBMASTER_USERNAME)))
            {
                var webMasterAccount = new ApplicationUser()
                {
                    UserName = STR_WEBMASTER_USERNAME,
                    Email = string.Format(STR_EMAIL_FORMAT, STR_WEBMASTER_USERNAME)
                };
                this.Create(webMasterAccount, STR_DEFAULT_PASSWORD);
                this.AddToRole(webMasterAccount.Id, SecurityRoles.WebsiteAdmins);
            }
        }

        #region Standard CRUD Operations
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public List<UserProfile> ListAllUsers()
        {
            var rm = new RoleManager();
            var result = from person in Users.ToList()
                         select new UserProfile()
                         {
                             UserId = person.Id,
                             UserName = person.UserName,
                             Email = person.Email,
                             EmailConfirmed = person.EmailConfirmed,
                             CustomerId = person.CustomerId,
                             EmployeeId = person.EmployeeId,
                             RoleMemberships = person.Roles.Select(r => rm.FindById(r.RoleId).Name)
                         };

            // Get any first/last names of users
            using (var context = new ToolsContext())
            {
                foreach (var person in result)
                    if (person.EmployeeId.HasValue)
                    {
                        person.FirstName = context.Employees.Find(person.EmployeeId).FirstName;
                        person.LastName = context.Employees.Find(person.EmployeeId).LastName;
                    }
            }

            return result.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        public void AddUser(UserProfile userInfo)
        {
            var userAccount = new ApplicationUser()
            {
                UserName = userInfo.UserName,
                Email = userInfo.Email
            };
            this.Create(userAccount, STR_DEFAULT_PASSWORD);
            foreach (var roleName in userInfo.RoleMemberships)
                this.AddToRole(userAccount.Id, roleName);
        }

        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public void RemoveUser(UserProfile userInfo)
        {
            if (userInfo.UserName == STR_WEBMASTER_USERNAME)
                throw new Exception("The webmaster account cannot be removed");
            this.Delete(this.FindById(userInfo.UserId));
        }
        #endregion

        #region Business Process Operations
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public List<UnregisteredUser> ListAllUnregsiteredUsers()
        {
            using (var context = new ToolsContext())
            {
                // Make an in-memory list of employees who have login accounts
                var registeredEmployees = (from emp in Users
                                           where emp.EmployeeId.HasValue
                                           select emp.EmployeeId).ToList();
                // Query employees who don't have login accounts.
                // Make it in-memory (.ToList()) for the next step of assigning usernames/emails
                var employees = (from emp in context.Employees
                                 where !registeredEmployees.Any(e => emp.EmployeeID == e)
                                 select new UnregisteredUser()
                                 {
                                     Id = emp.EmployeeID,
                                     Name = emp.FirstName,
                                     OtherName = emp.LastName,
                                     UserType = UnregisteredUserType.Employee
                                 }).ToList();
                // Assign employee usernames and emails
                foreach (var person in employees)
                {
                    person.AssignedUserName = string.Format(STR_USERNAME_FORMAT, person.Name, person.OtherName);
                    person.AssignedEmail = string.Format(STR_EMAIL_FORMAT, person.AssignedUserName);
                }

                // Make an in-memory list of customers who have login accounts
                var registeredOnlineCustomers = (from cust in Users
                                                 select cust.CustomerId).ToList();
                // Query customers who don't have login accounts.
                var customers = from cust in context.OnlineCustomers
                                where !registeredOnlineCustomers.Any(c => cust.OnlineCustomerID == c)
                                select new UnregisteredUser()
                                {
                                    Id = cust.OnlineCustomerID,
                                    UserType = UnregisteredUserType.Customer
                                };

                // Merge and return the results
                return employees.Union(customers).ToList();
            }
        }

        public void RegisterUser(UnregisteredUser userInfo)
        {
            // string randomPassword = Guid.NewGuid().ToString().Replace("-", "");
            var userAccount = new ApplicationUser()
            {
                UserName = userInfo.AssignedUserName,
                Email = userInfo.AssignedEmail
            };
            switch (userInfo.UserType)
            {
                case UnregisteredUserType.Customer:
                    userAccount.CustomerId = userInfo.Id;
                    break;
                case UnregisteredUserType.Employee:
                    userAccount.EmployeeId = userInfo.Id;
                    break;
            }

            this.Create(userAccount, STR_DEFAULT_PASSWORD); // or randomPassword
            switch (userInfo.UserType)
            {
                case UnregisteredUserType.Employee:
                    this.AddToRole(userAccount.Id, SecurityRoles.Staff);
                    break;
                case UnregisteredUserType.Customer:
                    this.AddToRole(userAccount.Id, SecurityRoles.RegisteredUsers);
                    break;
            }
        }
        #endregion
    }
}