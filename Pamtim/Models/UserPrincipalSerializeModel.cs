namespace Pamtim.Models
{
    public class UserPrincipalSerializeModel
    {

        public int ID_User { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public System.Guid ActivationCode { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
    }
}