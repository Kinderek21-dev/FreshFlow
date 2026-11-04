using FreshFlow.Data;
using FreshFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Include(o => o.Product)
                .Where(o => o.ClientId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Create(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null || product.StockQuantity <= 0)
            {
                TempData["Error"] = "Wybrany produkt jest aktualnie niedostępny.";
                return RedirectToAction("Index", "ClientPanel");
            }

            if (product.ExpirationDate.Year < 2020)
            {
                product.ExpirationDate = DateTime.Today.AddDays(14);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }

            ViewBag.Product = product;
            var model = new Order
            {
                ProductId = product.ProductId,
                QuantityKg = (int)product.MinimumOrderQuantity,
                PreferredDeliveryDate = DateTime.Today.AddDays(1)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            var product = await _context.Products.FindAsync(order.ProductId);
            if (product == null) return NotFound();

            if (product.ExpirationDate.Year < 2020)
            {
                product.ExpirationDate = DateTime.Today.AddDays(14);
            }

            if (order.QuantityKg < product.MinimumOrderQuantity)
            {
                ModelState.AddModelError("QuantityKg",
                    $"Minimalne zamówienie hurtowe dla tego towaru wynosi {product.MinimumOrderQuantity} kg.");
            }

            if (order.QuantityKg > product.StockQuantity)
            {
                ModelState.AddModelError("QuantityKg",
                    $"Brak wystarczającej ilości w magazynie. Dostępne: {product.StockQuantity} kg.");
            }

            if (order.PreferredDeliveryDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("PreferredDeliveryDate",
                    "Data dostawy nie może być z przeszłości.");
            }
            else if (order.PreferredDeliveryDate.Date > product.ExpirationDate.Date)
            {
                ModelState.AddModelError("PreferredDeliveryDate",
                    $"Wybrana data dostawy przekracza datę przydatności tej partii towaru ({product.ExpirationDate:dd.MM.yyyy}). Wybierz wcześniejszy termin.");
            }

            ModelState.Remove(nameof(order.ClientId));

            if (ModelState.IsValid)
            {
                order.ClientId = _userManager.GetUserId(User)!;
                order.TotalPriceNet = order.QuantityKg * product.BasePricePerKg;
                order.Status = OrderStatus.Nowe;
                order.CreatedAt = DateTime.Now;

                product.StockQuantity -= order.QuantityKg;

                _context.Orders.Add(order);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Zamówienie #{order.OrderId} na {product.Name} ({order.QuantityKg} kg) zostało przyjęte i przekazane do magazynu!";
                return RedirectToAction(nameof(MyOrders));
            }

            ViewBag.Product = product;
            return View(order);
        }
    }
}