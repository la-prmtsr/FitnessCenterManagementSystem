using Microsoft.AspNetCore.Identity;

namespace FitnessCenterManagementSystem.Models
{
    public class ApplicationUser :IdentityUser

    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int ? Age { get; set; }
        public string? Gender { get; set; }
        public double? HeightCm { get; set; }
        public double? WeightKg { get; set; }
    }
}
