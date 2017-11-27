using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Framework.BLL.Security
{
    public enum UnregisteredUserType { Undefined, Employee, Customer }
    public class UnregisteredUser
    {
        public int Id { get; set; }
        public UnregisteredUserType UserType { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public string AssignedUserName { get; set; }
        public string AssignedEmail { get; set; }
    }
}
