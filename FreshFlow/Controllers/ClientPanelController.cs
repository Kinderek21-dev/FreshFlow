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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsArchived && p.StockQuantity > 0)
                .Take(6)
                .ToListAsync();

            ViewBag.CurrentUser = user;
            return View(featuredProducts);
        }
    }
}