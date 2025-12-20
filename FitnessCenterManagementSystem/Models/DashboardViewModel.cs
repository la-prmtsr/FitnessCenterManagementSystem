namespace FitnessCenterManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalMembers { get; set; }       // Toplam Üye Sayısı
        public int TotalAppointments { get; set; }  // Toplam Randevu
        public int TodaysAppointments { get; set; } // Bugünkü Randevular
        public decimal TotalRevenue { get; set; }   // Toplam Ciro (Tahmini)
        public string TopTrainerName { get; set; }  // En Popüler Hoca

        // Dashboard'un altında son 5 randevuyu listelemek için:
        public List<Appointment> UpcomingAppointments { get; set; }
    }
}
