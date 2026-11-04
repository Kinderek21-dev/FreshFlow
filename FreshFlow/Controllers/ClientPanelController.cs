using FreshFlow.Data;
using FreshFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize]
    public class ClientPanelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientPanelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var outdatedProducts = await _context.Products
                .Where(p => p.ExpirationDate.Year < 2020)
                .ToListAsync();

            if (outdatedProducts.Any())
            {
                foreach (var p in outdatedProducts)
                {
                    p.ExpirationDate = DateTime.Today.AddDays(14);
                }
                await _context.SaveChangesAsync();
            }

            var user = await _userManager.GetUserAsync(User);
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsArchived && p.StockQuantity > 0)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.BarcodeSKU.Contains(search));
            }

            ViewBag.CurrentUser = user;
            ViewBag.CurrentSearch = search;
            return View(await query.ToListAsync());
        }
    }
}