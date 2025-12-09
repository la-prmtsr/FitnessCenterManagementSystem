namespace FitnessCenterManagementSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime StartAt{ get; set; }
        public DateTime EndAt { get; set; }

        public bool IsConfirmed { get; set; } = false;
        public decimal Price { get; set; }
    }
}