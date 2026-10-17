using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace FreshFlow.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana")]
        [StringLength(100)]
        [Display(Name = "Kategoria")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}