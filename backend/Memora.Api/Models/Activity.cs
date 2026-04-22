using Memora.Api.Models;
public class Activity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime Start { get; set; }
    public int MaxParticipants { get; set; }
    public int FacilityId { get; set; }
    public Facility Facility { get; set; } = null!;
    public List<UserActivity> UserActivities { get; set; } = new();
}