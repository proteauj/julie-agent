using Memora.Api.Models;
public class UserActivity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ActivityId { get; set; }
    public Activity Activity { get; set; }
    public DateTime RegisteredAt { get; set; }
}