using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;
using Memora.Api.Data;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly DataContext _context;
        public AppointmentsController(DataContext context)
        {
            _context = context;
        }

        // GET /api/appointments/mine - horaire de l'utilisateur connecté
        [HttpGet("mine")]
        public IActionResult GetMyAppointments()
        {
            string userEmail = User.Identity?.Name ?? User.FindFirst("email")?.Value;
            var user = _context.Users.Include(u => u.Appointments).FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return Unauthorized();
            return Ok(user.Appointments.OrderBy(a => a.Start));
        }

        // POST /api/appointments - création RDV
        [HttpPost]
        public IActionResult AddAppointment([FromBody] Appointment model)
        {
            string userEmail = User.Identity?.Name ?? User.FindFirst("email")?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            model.UserId = user.Id;
            _context.Appointments.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        // PUT /api/appointments/{id} - modification RDV
        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, [FromBody] Appointment model)
        {
            var entry = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (entry == null) return NotFound();
            // Optionnel : Vérifie utilisateur ici
            entry.Title = model.Title;
            entry.Description = model.Description;
            entry.Start = model.Start;
            entry.End = model.End;
            entry.Type = model.Type;
            _context.SaveChanges();
            return Ok(entry);
        }
    }
}