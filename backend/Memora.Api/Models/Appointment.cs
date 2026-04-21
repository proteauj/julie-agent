using System;

namespace Memora.Api.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  // Navigation
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = ""; // "visite", "médical", "perso", etc.
    }
}