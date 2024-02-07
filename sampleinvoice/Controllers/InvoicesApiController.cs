using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        // GET: api/Invoices(search)
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
                else // Load all invoices if search string is empty
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

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);

                if (invoice == null || invoice.IsDeleted)
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

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice, string invoiceItems)
        {
            try
            {
                if (_context.Invoices.Any(i => i.InvoiceNumber == invoice.InvoiceNumber))
                {
                    return Conflict("Invoice number already exists.");
                }

                var items = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoiceItems);

                _context.Add(invoice);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.InvoiceId = invoice.InvoiceId;
                    _context.InvoiceItems.Add(item);
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.InvoiceId }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to save changes. " + ex.Message);
            }
        }

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice, string invoiceItems)
        {
            if (id != invoice.InvoiceId)
            {
                return BadRequest();
            }

            try
            {
                var existingItems = _context.InvoiceItems.Where(item => item.InvoiceId == id);
                _context.InvoiceItems.RemoveRange(existingItems);

                var items = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoiceItems);

                _context.Update(invoice);

                foreach (var item in items)
                {
                    item.InvoiceId = invoice.InvoiceId;
                    _context.InvoiceItems.Add(item);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to save changes. " + ex.Message);
            }
        }

        // DELETE: api/Invoices/5
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

                _context.Entry(invoice).State = EntityState.Modified;
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