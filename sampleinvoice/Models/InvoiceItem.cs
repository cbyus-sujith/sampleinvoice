using System.ComponentModel.DataAnnotations;

namespace sampleinvoice.Models
{
    public class InvoiceItem
    {
        [Key]
        public int InvoiceItemId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string Product { get; set; }

        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be a positive value.")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        // Foreign key
        public int InvoiceNumber { get; set; }

        public Invoice Invoice { get; set; }
    }
}