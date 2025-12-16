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

    public class TrainerAvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainerAvailabilities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TrainerAvailabilities.Include(t => t.Trainer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrainerAvailabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }

            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Create
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name");
            return View();
        }

        // POST: TrainerAvailabilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrainerId,Day,Start,End")] TrainerAvailability trainerAvailability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrainerId,Day,Start,End")] TrainerAvailability trainerAvailability)
        {
            if (id != trainerAvailability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainerAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerAvailabilityExists(trainerAvailability.Id))
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
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }

            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability != null)
            {
                _context.TrainerAvailabilities.Remove(trainerAvailability);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerAvailabilityExists(int id)
        {
            return _context.TrainerAvailabilities.Any(e => e.Id == id);
        }
    }
}
