using Memora.Api.Models;
namespace Memora.Api.Models
{
    public class FacilityAdmin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FacilityId { get; set; }

        // Navigation properties
        public User? User { get; set; } = null!;
        public Facility? Facility { get; set; } = null!;
    }
}