using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Pamtim.Models
{
    [MetadataType(typeof(UserRegistrationViewModel))]
    public partial class UserRegistration
    {
        public string ConfirmPassword { get; set; }
    }


    public class UserRegistrationViewModel
    {
        [Display(Name = "Korisničko ime")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Obavezno!")]
        public string UserName { get; set; }

        [Display(Name = "Lozinka")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Obavezno!")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimalno dozvoljeno 6 znakova!")]
        public string Password { get; set; }

        [Display(Name = "Potvrdi lozinku")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Obavezno!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Ponovljena lozinka nije jednaka!")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Obavezno")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public System.Guid ActivationCode { get; set; }

        public class Varification : ValidationAttribute
        {
            private static string code = "8";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string input = value.ToString();
                if (code.Equals(input))
                    return ValidationResult.Success;
                return new ValidationResult("Netočno!");

            }
        }

        [Varification]
        [Display(Name = "Koliko je (2*3+2)?")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Obavezno!")]
        public string VerificationCode { get; set; }

        public int Active { get; set; }
    }
}