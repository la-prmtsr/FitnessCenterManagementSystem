using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bio { get; set; }

        public string Specialities { get; set; }

        public int FitnessCenterId { get; set; }
        public FitnessCenter FitnessCenter { get; set; }

        public ICollection<TrainerAvailability> Availabilities { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}