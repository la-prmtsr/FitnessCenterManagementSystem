using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagementSystem.Data;
using FitnessCenterManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenterManagementSystem.Controllers
{
    [Authorize] // Sadece giris yapabilenler girebilir
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var query = _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .AsQueryable();

            // Admin değilse sadece kendininkileri görsün
            if (!User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();
                query = query.Where(a => a.MemberId == user.Id);
            }

            // Verileri çek
            var allAppointments = await query.ToListAsync();

            // Bellekte Sıralama Yapıyoruz (Split Logic)
            var futureAppointments = allAppointments
                .Where(a => a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate) // Yakın tarih en üstte
                .ToList();

            var pastAppointments = allAppointments
                .Where(a => a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate) // En son geçen tarih en üstte (arşivin tepesinde)
                .ToList();

            // İki listeyi birleştir: Önce gelecekler, sonra geçmişler
            var sortedList = futureAppointments.Concat(pastAppointments).ToList();

            return View(sortedList);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // --- AJAX API: Hizmete göre hocaları getirir ---
        [HttpGet]
        public JsonResult GetTrainersByService(int serviceId)
        {
            var trainers = _context.Trainers
                .Where(t => t.ServiceId == serviceId)
                .Select(t => new {
                    id = t.Id,
                    fullName = t.FullName
                })
                .ToList();

            return Json(trainers);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            // Başlangıçta boş liste gönderiyoruz, JS dolduracak
            ViewData["TrainerId"] = new SelectList(new List<Trainer>(), "Id", "FullName");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentDate,ServiceId,TrainerId")] Appointment appointment)
        {
            // 1. Kullanıcıyı Belirle
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();
            appointment.MemberId = user.Id;

            appointment.Status = "Oluşturuldu";

            // 2. Gereksiz Validasyonları Temizle
            ModelState.Remove("Member");
            ModelState.Remove("MemberId");
            ModelState.Remove("Service");
            ModelState.Remove("Trainer");
            ModelState.Remove("Status");

            // -----------------------------------------------------------
            // 🔥 KRİTİK EKLENTİ BAŞLIYOR (Hoca Kontrolü) 🔥
            // -----------------------------------------------------------

            // Önce seçilen hocayı veritabanından çekiyoruz (Günlerini kontrol etmek için)
            var trainer = await _context.Trainers.FindAsync(appointment.TrainerId);

            if (trainer == null)
            {
                ModelState.AddModelError("", "Hata: Seçilen antrenör bulunamadı.");
            }
            else
            {
                // A) HOCA GÜN KONTROLÜ
                // AppointmentDate.DayOfWeek bize "Monday", "Tuesday" verir. (Senin DB de İngilizce olduğu için uyumlu)
                string dayName = appointment.AppointmentDate.DayOfWeek.ToString();

                // Eğer hocanın çalışma günlerinde bu gün YOKSA hata ver
                if (string.IsNullOrEmpty(trainer.WorkingDays) || !trainer.WorkingDays.Contains(dayName))
                {
                    ModelState.AddModelError("", $"Üzgünüz, {trainer.FullName} {dayName} günleri çalışmamaktadır.");
                }

                // B) HOCA SAAT KONTROLÜ
                TimeSpan randevuSaati = appointment.AppointmentDate.TimeOfDay;
                if (randevuSaati < trainer.StartTime || randevuSaati >= trainer.EndTime)
                {
                    ModelState.AddModelError("", $"{trainer.FullName} sadece {trainer.StartTime:hh\\:mm} - {trainer.EndTime:hh\\:mm} saatleri arasında hizmet vermektedir.");
                }
            }
            // -----------------------------------------------------------
            // 🔥 EKLENTİ BİTTİ 🔥
            // -----------------------------------------------------------

            // 3. Temel Kontroller (Geçmiş Tarih)
            if (appointment.AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarihe randevu alamazsınız.");
            }

            // Salon Kuralları
            var gymSettings = await _context.GymSettings.FirstOrDefaultAsync();
            TimeSpan salonAcilis = gymSettings?.OpenTime ?? new TimeSpan(9, 0, 0);
            TimeSpan salonKapanis = gymSettings?.CloseTime ?? new TimeSpan(22, 0, 0);
            string acikGunler = gymSettings?.OpenDays ?? "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday";
            string gunIsmi = appointment.AppointmentDate.DayOfWeek.ToString();

            if (!acikGunler.Contains(gunIsmi))
            {
                ModelState.AddModelError("", "Spor salonumuz seçtiğiniz gün hizmet vermemektedir.");
            }

            if (appointment.AppointmentDate.TimeOfDay < salonAcilis || appointment.AppointmentDate.TimeOfDay >= salonKapanis)
            {
                ModelState.AddModelError("", $"Spor salonumuz {salonAcilis:hh\\:mm} - {salonKapanis:hh\\:mm} saatleri arasında açıktır.");
            }

            // 4. Çakışma Kontrolü (Overlap Logic)
            if (ModelState.IsValid) // Yukarıdaki hatalar yoksa buraya gir
            {
                var selectedService = await _context.Services.FindAsync(appointment.ServiceId);
                if (selectedService == null)
                {
                    ModelState.AddModelError("", "Seçilen hizmet bulunamadı.");
                }
                else
                {
                    DateTime newStart = appointment.AppointmentDate;
                    DateTime newEnd = newStart.AddMinutes(selectedService.DurationMinutes);

                    var existingAppointments = await _context.Appointments
                        .Include(a => a.Service)
                        .Where(a => a.TrainerId == appointment.TrainerId &&
                                    a.AppointmentDate.Date == newStart.Date) // Sadece o günkü randevuları çek
                        .ToListAsync();

                    foreach (var existing in existingAppointments)
                    {
                        DateTime existingStart = existing.AppointmentDate;
                        DateTime existingEnd = existingStart.AddMinutes(existing.Service.DurationMinutes);

                        // Zaman Çakışması Formülü
                        if (newStart < existingEnd && newEnd > existingStart)
                        {
                            ModelState.AddModelError("", $"Seçtiğiniz saatte antrenör dolu. ({existingStart:HH:mm} - {existingEnd:HH:mm} arası dolu)");
                            break;
                        }
                    }
                }
            }

            // 5. Kayıt
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa formu tekrar doldur (Dropdownları kaybetmemek için)
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);

            // Eğer kullanıcı bir hizmet seçmişse, hoca listesini tekrar dolduruyoruz ki boş gelmesin
            if (appointment.ServiceId != 0)
            {
                var filteredTrainers = _context.Trainers
                        .Where(t => t.ServiceId == appointment.ServiceId)
                        .Select(t => new { Id = t.Id, FullName = t.FullName });
                ViewData["TrainerId"] = new SelectList(filteredTrainers, "Id", "FullName", appointment.TrainerId);
            }
            else
            {
                ViewData["TrainerId"] = new SelectList(new List<Trainer>(), "Id", "FullName");
            }

            return View(appointment);
        }

        // --- YENİ EKLENEN METOTLAR (ONAY SİSTEMİ İÇİN) ---

        // POST: Appointments/ConfirmAttendance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAttendance(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            // Sadece randevu sahibi veya Admin onaylayabilir
            if (appointment.MemberId != user.Id && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            appointment.Status = "Kesinleşti";
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/CancelAttendance/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAttendance(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (appointment.MemberId != user.Id && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            // Randevuyu siliyoruz
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // ----------------------------------------------------

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", appointment.MemberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppointmentDate,Status,ServiceId,TrainerId,MemberId")] Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", appointment.MemberId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
