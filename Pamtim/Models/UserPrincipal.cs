using System.Security.Principal;

namespace Pamtim.Models
{
    public class UserPrincipal : IUserPrincipal
    {
        public IIdentity Identity { get; private set; }

        public UserPrincipal(string name)
        {
            this.Identity = new GenericIdentity(name);
        }

        public bool IsInRole(string role) { return false; }

        public int ID_User { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.Guid ActivationCode { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }



    }
}