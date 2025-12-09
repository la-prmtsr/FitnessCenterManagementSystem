using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public int DurationInMinutes { get; set; }
        public decimal Price { get; set; }
        public int FitnessCenterId { get; set; }
        public FitnessCenter FitnessCenter { get; set; }
    }
}