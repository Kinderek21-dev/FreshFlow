using FreshFlow.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize(Roles = "Administrator,Kierownik,Magazynier")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = await _context.Products.CountAsync(p => !p.IsArchived);
            ViewBag.LowStockProducts = await _context.Products
                .Where(p => !p.IsArchived && p.StockQuantity < 100)
                .ToListAsync();
            return View();
        }
    }
}