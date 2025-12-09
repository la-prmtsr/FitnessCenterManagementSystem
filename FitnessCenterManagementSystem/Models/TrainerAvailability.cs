namespace FitnessCenterManagementSystem.Models
{
    public class TrainerAvailability
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}