using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Address { get; set; }
        public string OpeningHours { get; set; }

        //public string ContactInfo { get; set; }

        public ICollection<Service> Services { get; set; }
        public ICollection<Trainer> Trainers { get; set; }


    }
}
