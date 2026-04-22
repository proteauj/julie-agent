using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;
using Memora.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            var user = GetCurrentUser(includeAppointments: true);
            if (user == null) return Unauthorized();

            return Ok(user.Appointments.OrderBy(a => a.Start));
        }

        // POST /api/appointments - création RDV
        [HttpPost]
        public IActionResult AddAppointment([FromBody] Appointment model)
        {
            var user = GetCurrentUser();
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
            var currentUser = GetCurrentUser();
            if (currentUser == null) return Unauthorized();

            var entry = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (entry == null) return NotFound();

            // Vérifie que le RDV appartient bien à l'utilisateur connecté
            if (entry.UserId != currentUser.Id) return Forbid();

            entry.Title = model.Title;
            entry.Description = model.Description;
            entry.Start = model.Start;
            entry.End = model.End;
            entry.Type = model.Type;

            _context.SaveChanges();
            return Ok(entry);
        }

        private User? GetCurrentUser(bool includeAppointments = false)
        {
            var email =
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            IQueryable<User> query = _context.Users;

            if (includeAppointments)
                query = query.Include(u => u.Appointments);

            return query.FirstOrDefault(u => u.Email == email);
        }
    }
}