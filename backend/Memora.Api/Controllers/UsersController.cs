using Microsoft.AspNetCore.Mvc;
using Memora.Api.Data;
using Memora.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id) return BadRequest();
            var dbUser = await _context.Users.FindAsync(id);
            if (dbUser == null) return NotFound();
            dbUser.Email = user.Email;
            dbUser.Nom = user.Nom;
            dbUser.LanguePreferree = user.LanguePreferree;
            dbUser.Phone = user.Phone;
            dbUser.NotificationPreference = user.NotificationPreference;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var email =
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Nom,
                user.Role,
                user.LanguePreferree,
                user.Phone,
                user.NotificationPreference,
                user.FacilityId,
                user.FavoriteDoctorId
            });
        }

        // PATCH api/users/{id}/favorite-doctor
        [HttpPatch("{id}/favorite-doctor")]
        [Authorize]
        public IActionResult SetFavoriteDoctor(int id, [FromBody] int doctorId)
        {
            var user = _context.Users.Find(id);
            var doctor = _context.Doctors.Find(doctorId);
            if (user == null || doctor == null) return NotFound();
            user.FavoriteDoctorId = doctor.Id;
            _context.SaveChanges();
            return Ok();
        }
    }
}