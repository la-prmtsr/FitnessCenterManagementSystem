using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NuGet.DependencyResolver;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Appointment Status")]
        public string? Status { get; set; } = "Pending"; // Pending, Approved, Cancelled

        // Iliskiler

        // Which Service?
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }

        // Which trainer?
        public int TrainerId { get; set; }
        public virtual Trainer? Trainer { get; set; }

        //Which User? (Identity User ile baglayacak)
        public string? MemberId { get; set; }
        [ValidateNever]
        public virtual IdentityUser? Member { get; set; }
    }
}
