using FitnessCenterManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenterManagementSystem.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // Seed Roles (Admin ve Uye rollerini olusturur)
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Admin User (Projede istenilen kullaniciyi olusturu )
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();
            string adminEmail = "b231210575@sakarya.edu.tr";
            string adminPassword = "sau";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                //else
                //{
                //    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                //    throw new Exception("Kullanıcı oluşturulamadı: " + errors);
                //}
            }
        }

        // 2. YENI EKLENEN METOT (SALON AYARLARI)
        public static async Task SeedSettingsAsync(ApplicationDbContext context)
        {
            // Eger ayarlar tablosu bossa (Any false donerse) icine gir
            if (!context.GymSettings.Any())
            {
                var defaultSetting = new GymSetting
                {
                    OpenTime = new TimeSpan(9, 0, 0),  // 09:00
                    CloseTime = new TimeSpan(22, 0, 0), // 22:00
                    OpenDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"
                };

                context.GymSettings.Add(defaultSetting);
                await context.SaveChangesAsync();
            }
        }
    }
}
