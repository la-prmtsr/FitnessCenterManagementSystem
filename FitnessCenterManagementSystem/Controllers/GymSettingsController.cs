using FitnessCenterManagementSystem.Data;
using FitnessCenterManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin girebilir
    public class GymSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GymSettings/Edit
        // ID istemiyoruz çünkü zaten veritabanında sadece 1 satır ayar var.
        public async Task<IActionResult> Edit()
        {
            // İlk satırı getir
            var setting = await _context.GymSettings.FirstOrDefaultAsync();

            // Eğer veritabanı boşsa (DbSeeder çalışmadıysa), yeni bir tane oluşturup gönderelim
            if (setting == null)
            {
                setting = new GymSetting
                {
                    OpenTime = new TimeSpan(9, 0, 0),
                    CloseTime = new TimeSpan(22, 0, 0),
                    OpenDays = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"
                };
                _context.Add(setting);
                await _context.SaveChangesAsync();
            }

            return View(setting);
        }

        // POST: GymSettings/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OpenTime,CloseTime,OpenDays")] GymSetting gymSetting)
        {
            if (id != gymSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymSetting);
                    await _context.SaveChangesAsync();

                    // Başarılı mesajı gösterelim
                    ViewBag.Message = "Salon ayarları başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GymSettings.Any(e => e.Id == gymSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Kaydettikten sonra aynı sayfada kalsın
                return View(gymSetting);
            }
            return View(gymSetting);
        }
    }
}
