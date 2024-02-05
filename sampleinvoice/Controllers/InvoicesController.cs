using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using sampleinvoice.Data;
using sampleinvoice.Models;

namespace sampleinvoice.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices.ToListAsync();
            return View(invoices);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceNumber,Customer,Date,CustomerPO,Currency,PaymentMethod,VatPercentage,FreightCharge")] Invoice invoice, string invoiceItems)
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoiceItems);

                // Add the invoice to the database
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                // Associate each item with the invoice number and add it to the database
                foreach (var item in items)
                {
                    // Set the invoice number for each item
                    item.InvoiceNumber = invoice.InvoiceNumber;
                    _context.InvoiceItems.Add(item); // Modified line to add items to InvoiceItems DbSet
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
            }

            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.Include(i => i.InvoiceItems).FirstOrDefaultAsync(i => i.InvoiceNumber == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceNumber,Customer,Date,CustomerPO,Currency,PaymentMethod,VatPercentage,FreightCharge")] Invoice invoice, string invoiceItems)
        {
            if (id != invoice.InvoiceNumber)
            {
                return NotFound();
            }

            try
            {
                // Remove existing items associated with the invoice
                var existingItems = _context.InvoiceItems.Where(item => item.InvoiceNumber == id);
                _context.InvoiceItems.RemoveRange(existingItems);

                // Deserialize the incoming JSON string of invoice items
                var items = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoiceItems);

                // Update invoice details
                _context.Update(invoice);

                // Associate each item with the invoice number and add it to the database
                foreach (var item in items)
                {
                    // Set the invoice number for each item
                    item.InvoiceNumber = invoice.InvoiceNumber;
                    _context.InvoiceItems.Add(item);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.Include(i => i.InvoiceItems).FirstOrDefaultAsync(i => i.InvoiceNumber == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.InvoiceNumber == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}