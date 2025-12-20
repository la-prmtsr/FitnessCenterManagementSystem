using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fullname")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Expertise")]
        public string Expertise { get; set; } // Orn: Kilo Verme, Kas Kazandırma

        [Display(Name = "Working Days")]
        public string? WorkingDays { get; set; } // Null olabilir

        [Display(Name = "Start time")]
        public TimeSpan StartTime { get; set; } // Orn: 09:00

        [Display(Name = "End time")]
        public TimeSpan EndTime { get; set; }   // Orn: 18:00

        [Display(Name = "Services")]
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        //Iliski : Antrenorun randevulari
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
