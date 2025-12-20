using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Program Name")]
        public string Name { get; set; } // Orn: Yoga, Fitness

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Duration")]
        public int DurationMinutes { get; set; } // Orn: 60 minutes

        [Required]
        [Display(Name = "Price (TL)")]
        public decimal Price { get; set; }
    }
}
