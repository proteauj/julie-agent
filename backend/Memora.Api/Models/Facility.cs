using Memora.Api.Models;
namespace Memora.Api.Models
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";

        // Navigation properties
        public List<User> Seniors { get; set; } = new();
        public List<FacilityAdmin> FacilityAdmins { get; set; } = new();
    }
}