using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        public string? OpeningHours { get; set; }

        //public string ContactInfo { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();


    }
}
