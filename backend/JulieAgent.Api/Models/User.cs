using System.ComponentModel.DataAnnotations;

namespace JulieAgent.Api.Models
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
    }
}