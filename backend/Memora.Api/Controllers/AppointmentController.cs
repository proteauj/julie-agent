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
            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var appointments = _context.Appointments
                .Where(a => a.UserId == user.Id)
                .OrderBy(a => a.Start)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Start,
                    a.End,
                    a.Type,
                    a.UserId
                })
                .ToList();

            return Ok(appointments);
        }

        // POST /api/appointments - création RDV
        [HttpPost]
        public IActionResult AddAppointment([FromBody] AppointmentDto dto)
        {
            if (dto.Start == default)
                return BadRequest("La date de début est obligatoire.");

            if (dto.End.HasValue && dto.End <= dto.Start)
                return BadRequest("La date de fin doit être après la date de début.");

            var user = GetCurrentUser();
            if (user == null) return Unauthorized();

            var appointment = new Appointment
            {
                UserId = user.Id,
                Title = dto.Title,
                Description = dto.Description ?? "",
                Start = dto.Start,
                End = dto.End,
                Type = dto.Type
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return Ok(new
            {
                appointment.Id,
                appointment.Title,
                appointment.Description,
                appointment.Start,
                appointment.End,
                appointment.Type,
                appointment.UserId
            });
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
                User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            IQueryable<User> query = _context.Users;

            if (includeAppointments)
                query = query.Include(u => u.Appointments);

            return query.FirstOrDefault(u => u.Email == email);
        }
    }
}