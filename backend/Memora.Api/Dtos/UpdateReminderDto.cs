using System.ComponentModel.DataAnnotations;

namespace Memora.Api.Dtos
{
    public class UpdateReminderDto
    {
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

        [Required]
        [MaxLength(20)]
        public string Channel { get; set; } = "sms";

        public bool IsRecurring { get; set; } = false;

        [MaxLength(50)]
        public string? RecurrenceRule { get; set; }

        public bool IsDone { get; set; }

        public int? SourceId { get; set; }

        [MaxLength(30)]
        public string? SourceType { get; set; }
    }
}