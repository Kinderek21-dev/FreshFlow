using System.Text;
using FreshFlow.Data;
using FreshFlow.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Controllers
{
    [Authorize(Roles = "Administrator,Kierownik")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, bool onlyDelivered = false)
        {
            var start = fromDate ?? DateTime.Today.AddDays(-30);
            var end = toDate ?? DateTime.Today.AddDays(14);

            var query = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.PreferredDeliveryDate.Date >= start.Date && o.PreferredDeliveryDate.Date <= end.Date)
                .AsQueryable();

            if (onlyDelivered)
            {
                query = query.Where(o => o.Status == OrderStatus.Dostarczone);
            }

            var orders = await query.OrderByDescending(o => o.PreferredDeliveryDate).ToListAsync();

            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");
            ViewBag.OnlyDelivered = onlyDelivered;

            ViewBag.TotalOrdersCount = orders.Count;
            ViewBag.DeliveredCount = orders.Count(o => o.Status == OrderStatus.Dostarczone);
            ViewBag.InProgressCount = orders.Count(o => o.Status != OrderStatus.Dostarczone && o.Status != OrderStatus.Anulowane);
            ViewBag.TotalKgSold = orders.Sum(o => o.QuantityKg);
            ViewBag.TotalRevenueNet = orders.Sum(o => o.TotalPriceNet);
            ViewBag.TotalRevenueGross = orders.Sum(o =>
                o.TotalPriceNet * (1 + ((o.Product?.VatRate ?? 5) / 100m)));

            return View(orders);
        }

        public async Task<IActionResult> ExportCsv(DateTime? fromDate, DateTime? toDate, bool onlyDelivered = false)
        {
            var start = fromDate ?? DateTime.Today.AddDays(-30);
            var end = toDate ?? DateTime.Today.AddDays(14);

            var query = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .Where(o => o.PreferredDeliveryDate.Date >= start.Date && o.PreferredDeliveryDate.Date <= end.Date)
                .AsQueryable();

            if (onlyDelivered)
            {
                query = query.Where(o => o.Status == OrderStatus.Dostarczone);
            }

            var orders = await query.OrderByDescending(o => o.PreferredDeliveryDate).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Nr Zamowienia;Data Zlozenia;Termin Dostawy;Klient (Firma);NIP;Produkt;Kod SKU;Ilosc (kg);Kwota Netto (PLN);VAT (%);Kwota Brutto (PLN);Status Dostawy");

            foreach (var o in orders)
            {
                var vatRate = o.Product?.VatRate ?? 5;
                var gross = o.TotalPriceNet * (1 + (vatRate / 100m));
                var company = o.Client?.CompanyName ?? o.Client?.Email ?? "Klient";
                var nip = o.Client?.NIP ?? "-";

                sb.AppendLine($"#{o.OrderId};{o.CreatedAt:dd.MM.yyyy HH:mm};{o.PreferredDeliveryDate:dd.MM.yyyy};{company};{nip};{o.Product?.Name};{o.Product?.BarcodeSKU};{o.QuantityKg};{o.TotalPriceNet:0.00};{vatRate}%;{gross:0.00};{o.Status}");
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var body = Encoding.UTF8.GetBytes(sb.ToString());
            var finalBytes = preamble.Concat(body).ToArray();

            string fileName = $"Raport_Sprzedazy_FreshFlow_{start:yyyyMMdd}_{end:yyyyMMdd}.csv";
            return File(finalBytes, "text/csv", fileName);
        }
    }
}