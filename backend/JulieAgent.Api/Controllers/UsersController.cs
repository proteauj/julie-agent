using Microsoft.AspNetCore.Mvc;
using JulieAgent.Api.Data;
using JulieAgent.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JulieAgent.Api.Controllers
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
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}