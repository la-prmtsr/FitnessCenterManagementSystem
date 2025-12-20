using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagementSystem.Models
{
    public class GymSetting
    {
        public int Id { get; set; }

        [Display(Name = "Opening Time")]
        public TimeSpan OpenTime { get; set; } // Orn: 08:00

        [Display(Name = "Closing Time")]
        public TimeSpan CloseTime { get; set; } // Orn: 23:00

        [Display(Name = "Opening Days")]
        public string OpenDays { get; set; }
    }
}
