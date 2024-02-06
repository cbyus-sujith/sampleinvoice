using System.ComponentModel.DataAnnotations;

namespace sampleinvoice.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Invoice number is required.")]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        public string Customer { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Customer PO is required.")]
        public string CustomerPO { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "VAT percentage is required.")]
        [Range(0, 100, ErrorMessage = "VAT percentage must be between 0 and 100.")]
        public decimal VatPercentage { get; set; }

        [Required(ErrorMessage = "Freight charge is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Freight charge must be a positive value.")]
        public decimal FreightCharge { get; set; }

        // Navigation property to link products with this invoice
        public List<InvoiceItem> InvoiceItems { get; set; }

        // Delete flag
        public bool IsDeleted { get; set; } = false;
    }
}