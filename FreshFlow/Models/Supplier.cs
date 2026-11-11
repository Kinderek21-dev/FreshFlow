using System.ComponentModel.DataAnnotations;

namespace FreshFlow.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Nazwa gospodarstwa / kontrahenta jest wymagana")]
        [StringLength(120)]
        [Display(Name = "Nazwa gospodarstwa / firmy")]
        public string FarmName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer NIP jest wymagany")]
        [RegularExpression(@"^\d{10}$|^\d{3}-\d{3}-\d{2}-\d{2}$|^\d{3}-\d{2}-\d{2}-\d{3}$", ErrorMessage = "Podaj poprawny 10-cyfrowy numer NIP")]
        [Display(Name = "NIP")]
        public string NIP { get; set; } = string.Empty;

        [Required(ErrorMessage = "Imię i nazwisko właściciela/osoby kontaktowej jest wymagane")]
        [Display(Name = "Osoba kontaktowa (Rolnik)")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon kontaktowy jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail")]
        [Display(Name = "Adres e-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj region / miejscowość gospodarstwa")]
        [Display(Name = "Region / Miejscowość")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj specjalizację upraw")]
        [Display(Name = "Specjalizacja (np. Warzywa szklarniowe, Sady jabłkowe)")]
        public string Specialization { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}