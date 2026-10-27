using System.ComponentModel.DataAnnotations;

namespace FreshFlow.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwa firmy jest wymagana")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "NIP jest wymagany")]
        public string NIP { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Hasła nie są identyczne")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}