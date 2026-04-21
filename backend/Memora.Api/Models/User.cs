using System.ComponentModel.DataAnnotations;

namespace Memora.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Nom { get; set; }

        [MaxLength(10)]
        public string LanguePreferree { get; set; } = "fr";

        // 🔒 Ajoute ceci pour l'auth (pas relayé au front)
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "senior";
        public string Phone { get; set; } = "";

        // Facility membership
        public int? FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public string NotificationPreference { get; set; } = "sms"; // ou "email"
        public int? FavoriteDoctorId { get; set; }
        public Doctor? FavoriteDoctor { get; set; }

        public List<Appointment> Appointments { get; set; } = new();
    }
}