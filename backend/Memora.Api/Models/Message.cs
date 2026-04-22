namespace Memora.Api.Models
{
    public class Message
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Role { get; set; } = "user"; // user / assistant

        public string Content { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}