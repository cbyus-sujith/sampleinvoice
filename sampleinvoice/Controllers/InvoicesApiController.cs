using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sampleinvoice.Data;
using sampleinvoice.Models;

namespace sampleinvoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            try
            {
                var invoices = await _context.Invoices.Where(i => !i.IsDeleted).ToListAsync();
                return invoices;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetSearchInvoices(string searchString)
        {
            try
            {
                var invoices = _context.Invoices.Where(i => !i.IsDeleted);

                if (!string.IsNullOrEmpty(searchString))
                {
                    invoices = invoices.Where(i =>
                        i.InvoiceNumber.Contains(searchString) ||
                        i.Customer.Contains(searchString) ||
                        i.CustomerPO.Contains(searchString));
                }
                else
                {
                    return await _context.Invoices.Where(i => !i.IsDeleted).ToListAsync();
                }

                return await invoices.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (invoice == null)
                {
                    return NotFound();
                }

                return invoice;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("CreateInvoice")]
        public IActionResult CreateInvoice([FromBody] Invoice invoice)
        {
            try
            {
                _context.Invoices.Add(invoice);
                _context.SaveChanges();

                return Ok(invoice); // Return the newly created invoice object
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] Invoice invoice)
        {
            try
            {
                var existingInvoice = await _context.Invoices.Include(i => i.InvoiceItems).FirstOrDefaultAsync(i => i.InvoiceId == id);

                if (existingInvoice == null)
                {
                    return NotFound("Invoice not found.");
                }
                _context.InvoiceItems.RemoveRange(existingInvoice.InvoiceItems);
                existingInvoice.Date = invoice.Date;
                existingInvoice.Customer = invoice.Customer;
                existingInvoice.CustomerPO = invoice.CustomerPO;
                existingInvoice.Currency = invoice.Currency;
                existingInvoice.PaymentMethod = invoice.PaymentMethod;
                existingInvoice.VatPercentage = invoice.VatPercentage;
                existingInvoice.FreightCharge = invoice.FreightCharge;
                existingInvoice.InvoiceItems = invoice.InvoiceItems;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to save changes. " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to delete invoice. " + ex.Message);
            }
        }
    }
}