namespace Memora.Api.Models;

public class UpdateProfileDto
{
    public string? Nom { get; set; }
    public string? Phone { get; set; }
    public string LanguePreferree { get; set; } = "fr";
    public string NotificationPreference { get; set; } = "sms";
    public int? FavoriteDoctorId { get; set; }
}