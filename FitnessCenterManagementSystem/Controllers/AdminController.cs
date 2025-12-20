using FitnessCenterManagementSystem.Data;
using FitnessCenterManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Patron Girebilir
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Verileri Hazırla
            var model = new DashboardViewModel();

            // A) Toplam Üye Sayısı (DÜZELTİLDİ)
            // Veritabanındaki tüm kullanıcıları sayıyoruz
            var allUsersCount = await _userManager.Users.CountAsync();

            // Admin olanların sayısını buluyoruz
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            // Toplamdan Adminleri çıkarırsak geriye Üyeler kalır
            // (Bu sayede rolü atanmamış olsa bile üye sayılır)
            model.TotalMembers = allUsersCount - admins.Count;

            // B) Toplam Randevu Sayısı
            model.TotalAppointments = await _context.Appointments.CountAsync();

            // C) Bugünkü Randevu Sayısı
            model.TodaysAppointments = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == DateTime.Today);

            // D) Toplam Ciro Hesabı (Hizmet Fiyatı x Randevu Sayısı)
            // Not: İptal edilenler silindiği için hesaba katılmaz, bu iyi bir şey.
            model.TotalRevenue = await _context.Appointments
                .Include(a => a.Service)
                .SumAsync(a => a.Service.Price);

            // E) En Popüler Hoca (En çok randevusu olan)
            var topTrainerData = await _context.Appointments
                .GroupBy(a => a.TrainerId)
                .Select(g => new { TrainerId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            if (topTrainerData != null)
            {
                var trainer = await _context.Trainers.FindAsync(topTrainerData.TrainerId);
                model.TopTrainerName = trainer?.FullName ?? "Bilinmiyor";
            }
            else
            {
                model.TopTrainerName = "Henüz Yok";
            }

            // F) Yaklaşan Son 5 Randevu (Listeleme için)
            model.UpcomingAppointments = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Where(a => a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .Take(5)
                .ToListAsync();

            return View(model);
        }
    }
}
