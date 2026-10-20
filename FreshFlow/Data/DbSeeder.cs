using FreshFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FreshFlow.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Administrator", "Kierownik", "Magazynier", "Dostawca", "Klient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@freshflow.pl") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@freshflow.pl",
                    Email = "admin@freshflow.pl",
                    FirstName = "Marcin",
                    LastName = "Kowalski",
                    CompanyName = "FreshFlow Centrala",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Haslo123!");
                await userManager.AddToRolesAsync(admin, new[] { "Administrator", "Kierownik" });
            }

            if (await userManager.FindByEmailAsync("klient@restauracja.pl") == null)
            {
                var client = new ApplicationUser
                {
                    UserName = "klient@restauracja.pl",
                    Email = "klient@restauracja.pl",
                    FirstName = "Anna",
                    LastName = "Nowak",
                    CompanyName = "Bistro Zielony Talerz",
                    NIP = "525-000-11-22",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(client, "Haslo123!");
                await userManager.AddToRoleAsync(client, "Klient");
            }

            if (!context.Categories.Any())
            {
                var warzywa = new Category { Name = "Warzywa" };
                var owoce = new Category { Name = "Owoce" };
                context.Categories.AddRange(warzywa, owoce);
                await context.SaveChangesAsync();

                context.Products.AddRange(
                    new Product
                    {
                        Name = "Pomidor Malinowy",
                        BarcodeSKU = "WARZ-POM-001",
                        CategoryId = warzywa.CategoryId,
                        OriginCountry = "Polska",
                        BasePricePerKg = 8.50m,
                        MinimumOrderQuantity = 10,
                        VatRate = 5,
                        StockQuantity = 450,
                        ImageUrl = "https://images.unsplash.com/photo-1592924357228-91a4daadcfea?w=600"
                    },
                    new Product
                    {
                        Name = "Jabłko Gala",
                        BarcodeSKU = "OWO-JAB-002",
                        CategoryId = owoce.CategoryId,
                        OriginCountry = "Polska",
                        BasePricePerKg = 3.90m,
                        MinimumOrderQuantity = 25,
                        VatRate = 5,
                        StockQuantity = 1200,
                        ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=600"
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}