using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreshFlow.Models
{
    public enum OrderStatus
    {
        Nowe = 0,              
        WKompletacji = 1,     
        GotoweDlaDostawcy = 2, 
        WDrodze = 3,           
        Dostarczone = 4,       
        Anulowane = 5
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [ForeignKey(nameof(ClientId))]
        public ApplicationUser? Client { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Podaj zamawianą ilość w kg")]
        [Range(1, 100000, ErrorMessage = "Ilość musi być większa od 0")]
        [Display(Name = "Ilość (kg)")]
        public int QuantityKg { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Kwota całkowita netto (zł)")]
        public decimal TotalPriceNet { get; set; }

        [Required(ErrorMessage = "Wybierz preferowaną datę dostawy")]
        [DataType(DataType.Date)]
        [Display(Name = "Preferowana data dostawy")]
        public DateTime PreferredDeliveryDate { get; set; }

        [Required(ErrorMessage = "Wybierz przedział godzinowy dostawy")]
        [Display(Name = "Preferowana godzina dostawy")]
        public string PreferredDeliveryTimeWindow { get; set; } = "06:00 - 09:00";

        [Required(ErrorMessage = "Podaj adres dostawy")]
        [Display(Name = "Adres dostawy")]
        public string DeliveryAddress { get; set; } = string.Empty;

        public OrderStatus Status { get; set; } = OrderStatus.Nowe;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}