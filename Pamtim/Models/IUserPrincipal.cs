using System.Security.Principal;

namespace Pamtim.Models
{
    public interface IUserPrincipal : IPrincipal
    {

        int ID_User { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        System.Guid ActivationCode { get; set; }
        string Password { get; set; }
        bool Active { get; set; }
   
    }
}
