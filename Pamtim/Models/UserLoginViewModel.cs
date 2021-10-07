using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Pamtim.Models
{
    public class UserLoginViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserName je obavezan!")]
        [Display(Name = "Korisničko ime")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password je obavezan!")]
        [Display(Name = "Lozinka")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int ID_User { get; set; }
        
        public string Email { get; set; }
        public System.Guid ActivationCode { get; set; }
        
        public bool Active { get; set; }
    }
}