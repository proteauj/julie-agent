using Memora.Api.Models;
namespace Memora.Api.Models
{
    public class FacilityAdmin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FacilityId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Facility? Facility { get; set; }
    }
}