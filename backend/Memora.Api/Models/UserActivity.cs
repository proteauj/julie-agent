using Memora.Api.Models;
public class UserActivity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
}