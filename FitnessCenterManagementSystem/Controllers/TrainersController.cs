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

namespace FitnessCenterManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            // Include(t => t.Service) ekleyerek Service tablosunu da çekiyoruz
            var trainers = _context.Trainers.Include(t => t.Service);
            return View(await trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Include(t => t.Service) EKLENDİ!
            var trainer = await _context.Trainers
                .Include(t => t.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            // Hizmetleri Dropdown icin sayfaya gonderiyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Expertise,WorkingDays,StartTime,EndTime,ServiceId")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Hata varsa listeyi tekrar gonder
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            // DROP-DOWN DOLDURMA 
            // Bu sayede sayfa acildiginda hocanin mevcut hizmeti secili gelir.
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);

            return View(trainer);
        }

        // POST: Trainers/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Expertise,WorkingDays,StartTime,EndTime,ServiceId")] Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa listeyi tekrar doldur (Kaybolmasin diye)
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Include(t => t.Service) EKLENDİ!
            var trainer = await _context.Trainers
                .Include(t => t.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // ESKİSİ: var trainer = await _context.Trainers.FindAsync(id);
            // YENİSİ: Include ekleyerek çekiyoruz ki hata verirse Hizmet adı kaybolmasın.
            var trainer = await _context.Trainers
                .Include(t => t.Service) // <-- BU SATIR EKSİKTİ
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null)
            {
                return NotFound();
            }

            // --- RANDEVU KONTROLÜ ---
            bool hasFutureAppointments = await _context.Appointments
                .AnyAsync(a => a.TrainerId == id && a.AppointmentDate >= DateTime.Now);

            if (hasFutureAppointments)
            {
                // Hata mesajı gönder
                ViewBag.Error = "DİKKAT: Bu antrenörün bekleyen randevuları var! Önce randevuları iptal etmelisiniz.";

                // Artık 'trainer' değişkeninin içinde Service bilgisi dolu olduğu için
                // sayfaya geri döndüğümüzde Hizmet adı yazacak.
                return View("Delete", trainer);
            }
            // ------------------------

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}
