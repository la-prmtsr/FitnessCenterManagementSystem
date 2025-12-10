using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string? Surname { get; set; }
        [StringLength(500)]
        public string? Bio { get; set; }

        public string? Specialities { get; set; }

        public int FitnessCenterId { get; set; }
        public FitnessCenter FitnessCenter { get; set; }

        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}