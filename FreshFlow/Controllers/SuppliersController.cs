using FreshFlow.Data;
using FreshFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize(Roles = "Administrator,Kierownik,Magazynier")]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.Suppliers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.FarmName.Contains(search) ||
                    s.NIP.Contains(search) ||
                    s.Specialization.Contains(search) ||
                    s.Region.Contains(search));
            }

            ViewBag.CurrentSearch = search;
            var suppliers = await query.OrderBy(s => s.FarmName).ToListAsync();
            return View(suppliers);
        }

        [Authorize(Roles = "Administrator,Kierownik")]
        public IActionResult Create()
        {
            return View(new Supplier());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Kierownik")]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (await _context.Suppliers.AnyAsync(s => s.NIP == supplier.NIP))
            {
                ModelState.AddModelError("NIP", "Dostawca o podanym numerze NIP już istnieje w bazie.");
            }

            if (ModelState.IsValid)
            {
                supplier.CreatedAt = DateTime.Now;
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Dodano nowego dostawcę: {supplier.FarmName}.";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        [Authorize(Roles = "Administrator,Kierownik")]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Kierownik")]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zaktualizowano dane dostawcy: {supplier.FarmName}.";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Kierownik")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Dostawca został usunięty z bazy.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReceiveSupply(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            ViewBag.Supplier = supplier;
            ViewBag.Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "ProductId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReceiveSupply(int supplierId, int productId, decimal deliveredKg, DateTime newExpirationDate)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            var product = await _context.Products.FindAsync(productId);

            if (supplier == null || product == null) return NotFound();

            if (deliveredKg <= 0)
            {
                ModelState.AddModelError("", "Ilość przyjętego towaru musi być większa od 0 kg.");
            }

            if (newExpirationDate.Date <= DateTime.Today)
            {
                ModelState.AddModelError("", "Data przydatności nowej partii musi być w przyszłości.");
            }

            if (ModelState.IsValid)
            {
                product.StockQuantity += deliveredKg;
                product.ExpirationDate = newExpirationDate;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Przyjęto dostawę od '{supplier.FarmName}': +{deliveredKg} kg produktu '{product.Name}' (nowy stan: {product.StockQuantity} kg, ważność do: {newExpirationDate:dd.MM.yyyy}).";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Supplier = supplier;
            ViewBag.Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "ProductId", "Name", productId);
            return View();
        }
    }
}