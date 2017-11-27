using Microsoft.AspNet.Identity.EntityFramework;

namespace Tools.Framework.BLL.Security
{
    // You can add User data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public int CustomerId { get; set; }
    }
}