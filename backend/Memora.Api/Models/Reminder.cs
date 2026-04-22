using System.ComponentModel.DataAnnotations;

namespace Memora.Api.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [MaxLength(30)]
        public string Type { get; set; } = "general";
        // general | medication | appointment | activity | meal | sleep

        [Required]
        [MaxLength(20)]
        public string Channel { get; set; } = "sms";
        // sms | email | both

        public bool IsDone { get; set; } = false;

        public DateTime? CompletedAt { get; set; } = null!;

        public bool IsRecurring { get; set; } = false;

        [MaxLength(50)]
        public string? RecurrenceRule { get; set; }
        // daily | weekly | monthly | custom plus tard

        public int? SourceId { get; set; }

        [MaxLength(30)]
        public string? SourceType { get; set; }
        // appointment | activity | medication | meal | sleep

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}