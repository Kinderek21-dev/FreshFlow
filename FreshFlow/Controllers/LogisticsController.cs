using FreshFlow.Data;
using FreshFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize(Roles = "Administrator,Kierownik,Magazynier,Dostawca")]
    public class LogisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LogisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator,Kierownik,Magazynier")]
        public async Task<IActionResult> WarehousePanel()
        {
            var orders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .OrderBy(o => o.PreferredDeliveryDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Kierownik,Magazynier")]
        public async Task<IActionResult> UpdateWarehouseStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Zamówienie #{orderId} zmieniło status na: {newStatus}.";
            }
            return RedirectToAction(nameof(WarehousePanel));
        }

        [Authorize(Roles = "Administrator,Kierownik,Dostawca")]
        public async Task<IActionResult> DeliveryPanel()
        {
            var ordersForCourier = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.Status == OrderStatus.GotoweDlaDostawcy
                         || o.Status == OrderStatus.WDrodze
                         || o.Status == OrderStatus.Dostarczone)
                .OrderBy(o => o.PreferredDeliveryDate)
                .ToListAsync();

            return View(ordersForCourier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Kierownik,Dostawca")]
        public async Task<IActionResult> UpdateDeliveryStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Status dostawy zamówienia #{orderId} zaktualizowany na: {newStatus}.";
            }
            return RedirectToAction(nameof(DeliveryPanel));
        }
    }
}