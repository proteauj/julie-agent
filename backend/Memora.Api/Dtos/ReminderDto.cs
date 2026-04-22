namespace Memora.Api.Dtos
{
    public class ReminderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrenceRule { get; set; }
        public int? SourceId { get; set; }
        public string? SourceType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}