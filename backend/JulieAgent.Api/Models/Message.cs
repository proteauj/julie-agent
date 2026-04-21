using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JulieAgent.Api.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = ""; // Ou int si ID user numérique
        [Required]
        public string Role { get; set; } = "user"; // "user" ou "assistant"
        [Required]
        public string Text { get; set; } = "";
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}