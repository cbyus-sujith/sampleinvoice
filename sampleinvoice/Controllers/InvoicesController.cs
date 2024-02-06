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

        public async Task<IActionResult> Index(string searchString)
        {
            var invoices = _context.Invoices.Where(i => !i.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                invoices = invoices.Where(i =>
                    i.InvoiceNumber.Contains(searchString) ||
                    i.Customer.Contains(searchString) ||
                    i.CustomerPO.Contains(searchString));
            }

            var invoiceList = await invoices.ToListAsync();
            return View(invoiceList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceNumber,Customer,Date,CustomerPO,Currency,PaymentMethod,VatPercentage,FreightCharge")] Invoice invoice, string invoiceItems)
        {
            try
            {
                var items = JsonConvert.DeserializeObject<List<InvoiceItem>>(invoiceItems);

                _context.Add(invoice);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    item.InvoiceId = invoice.InvoiceId;
                    _context.InvoiceItems.Add(item);
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.Include(i => i.InvoiceItems).FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceNumber,Customer,Date,CustomerPO,Currency,PaymentMethod,VatPercentage,FreightCharge")] Invoice invoice, string invoiceItems)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
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

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.Include(i => i.InvoiceItems).FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            invoice.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}