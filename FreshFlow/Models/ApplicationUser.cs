using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FreshFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Nazwa firmy")]
        public string? CompanyName { get; set; }

        [Display(Name = "NIP")]
        public string? NIP { get; set; }

        public bool IsBlocked { get; set; } = false;
    }
}