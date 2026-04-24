namespace Memora.Api.Models;

public class AppointmentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public string Type { get; set; } = "personal";
}