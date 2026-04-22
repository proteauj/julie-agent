using Memora.Api.Models;
public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Specialty { get; set; } = "";
    public string BookingUrl { get; set; } = "";
    public int FacilityId { get; set; }
    public Facility Facility { get; set; } = null!;
    public List<User> Users { get; set; } = new(); // Users ayant "FavoriteDoctorId" pointant ici
}