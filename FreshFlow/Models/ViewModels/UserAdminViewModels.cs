using System.ComponentModel.DataAnnotations;

namespace FreshFlow.Models.ViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? NIP { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
    }

    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format e-mail")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nazwa firmy (opcjonalnie)")]
        public string? CompanyName { get; set; }

        [Display(Name = "NIP (opcjonalnie)")]
        public string? NIP { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(6, ErrorMessage = "Minimum 6 znaków")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło początkowe")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wybierz rolę")]
        [Display(Name = "Rola w systemie")]
        public string Role { get; set; } = "Klient";
    }

    public class UserEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Imię jest wymagane")]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format e-mail")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nazwa firmy")]
        public string? CompanyName { get; set; }

        [Display(Name = "NIP")]
        public string? NIP { get; set; }

        [Required(ErrorMessage = "Wybierz rolę")]
        [Display(Name = "Przypisana rola")]
        public string Role { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Nowe hasło musi mieć min. 6 znaków")]
        [Display(Name = "Nowe hasło (zostaw puste, aby nie zmieniać)")]
        public string? NewPassword { get; set; }

        [Display(Name = "Konto zablokowane")]
        public bool IsBlocked { get; set; }
    }
}