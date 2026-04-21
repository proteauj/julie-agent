public class Rappel
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Texte { get; set; } = "";
    public DateTime Date { get; set; }
    public bool Fait { get; set; } = false;
}