using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFlow.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Nazwa produktu jest wymagana")]
        [StringLength(150)]
        [Display(Name = "Nazwa produktu")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kod kreskowy / SKU jest wymagany")]
        [StringLength(50)]
        [Display(Name = "Kod kreskowy (SKU)")]
        public string BarcodeSKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wybierz kategorię")]
        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Kraj pochodzenia jest wymagany")]
        [Display(Name = "Kraj pochodzenia")]
        public string OriginCountry { get; set; } = "Polska";

        [Required(ErrorMessage = "Cena podstawowa jest wymagana")]
        [Range(0.01, 10000.00, ErrorMessage = "Cena musi być większa od 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Cena podstawowa (zł/kg)")]
        public decimal BasePricePerKg { get; set; }

        [Required(ErrorMessage = "Minimalna ilość zamówienia jest wymagana")]
        [Range(1, 5000, ErrorMessage = "Minimalna ilość musi wynosić co najmniej 1 kg")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Min. zamówienie (kg)")]
        public decimal MinimumOrderQuantity { get; set; } = 10;

        [Required(ErrorMessage = "Stawka VAT jest wymagana")]
        [Range(0, 100, ErrorMessage = "Podaj poprawną stawkę VAT (%)")]
        [Display(Name = "Stawka VAT (%)")]
        public int VatRate { get; set; } = 5;

        [Display(Name = " Stan magazynowy (kg)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockQuantity { get; set; } = 100;

        [Display(Name = "URL zdjęcia")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Zarchiwizowany")]
        public bool IsArchived { get; set; } = false;
    }
}