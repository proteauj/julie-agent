public class Reminder
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Date { get; set; }
    public bool Done { get; set; } = false;
}