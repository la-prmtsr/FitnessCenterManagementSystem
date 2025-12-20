using FitnessCenterManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GymApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ANTRENÖRLERİ GETİRME
        // GET: api/GymApi/Trainers
        [HttpGet("Trainers")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Service) // Service tablosunu dahil et
                .Select(t => new
                {
                    id = t.Id,
                    fullName = t.FullName,
                    expertise = t.Expertise,
                    service = t.Service.Name, // Sadece ismini string olarak alıyoruz
                    workingDays = t.WorkingDays
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // 2. BELİRLİ BİR HİZMETE GÖRE FİLTRELEME
        // GET: api/GymApi/TrainersByService/Pilates
        [HttpGet("TrainersByService/{serviceName}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainersByService(string serviceName)
        {
            var trainers = await _context.Trainers
                .Include(t => t.Service)
                .Where(t => t.Service.Name.Contains(serviceName))
                .Select(t => new
                {
                    id = t.Id,
                    fullName = t.FullName,
                    expertise = t.Expertise,
                    service = t.Service.Name,
                    workingDays = t.WorkingDays
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // 3. İSTATİSTİK: HOCALARIN TOPLAM RANDEVU SAYISI (YENİ EKLENDİ)
        // GET: api/GymApi/Stats
        [HttpGet("Stats")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainerStats()
        {
            // LINQ GroupBy ve Count kullanımı
            var stats = await _context.Trainers
                .Select(t => new
                {
                    id = t.Id,
                    fullName = t.FullName,
                    service = t.Service.Name,
                    // Bu hocanın appointments tablosundaki kaydını say
                    totalAppointments = _context.Appointments.Count(a => a.TrainerId == t.Id)
                })
                .OrderByDescending(x => x.totalAppointments) // En çok randevusu olan üstte
                .ToListAsync();

            return Ok(stats);
        }

    }
}
