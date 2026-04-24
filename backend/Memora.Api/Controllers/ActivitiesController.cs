using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Memora.Api.Data;
using Memora.Api.Models;
using System.Security.Claims;

namespace Memora.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/activities")]
public class ActivitiesController : ControllerBase
{
    private readonly DataContext _context;

    public ActivitiesController(DataContext context)
    {
        _context = context;
    }

    private User? GetCurrentUser()
    {
        var email =
            User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value
            ?? User.FindFirst("sub")?.Value;

        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    [HttpGet]
    public IActionResult GetActivities()
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var activities = _context.Activities
            .OrderBy(a => a.Start)
            .Select(a => new
            {
                a.Id,
                a.Name,
                a.Description,
                a.Start,
                a.MaxParticipants,
                a.FacilityId,
                registeredCount = _context.UserActivities.Count(ua => ua.ActivityId == a.Id),
                isRegistered = _context.UserActivities.Any(ua =>
                    ua.ActivityId == a.Id && ua.UserId == user.Id),
                canRegister = _context.UserActivities.Count(ua => ua.ActivityId == a.Id) < a.MaxParticipants
            })
            .ToList();

        return Ok(activities);
    }

    [HttpPost("{id}/register")]
    public IActionResult Register(int id)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var activity = _context.Activities.FirstOrDefault(a => a.Id == id);
        if (activity == null) return NotFound();

        var alreadyRegistered = _context.UserActivities.Any(ua =>
            ua.ActivityId == id && ua.UserId == user.Id);

        if (alreadyRegistered)
            return BadRequest("Déjà inscrit à cette activité.");

        var registeredCount = _context.UserActivities.Count(ua => ua.ActivityId == id);
        if (registeredCount >= activity.MaxParticipants)
            return BadRequest("Cette activité est complète.");

        _context.UserActivities.Add(new UserActivity
        {
            ActivityId = id,
            UserId = user.Id,
            RegisteredAt = DateTime.UtcNow
        });

        _context.SaveChanges();

        return Ok();
    }

    [HttpDelete("{id}/register")]
    public IActionResult Unregister(int id)
    {
        var user = GetCurrentUser();
        if (user == null) return Unauthorized();

        var registration = _context.UserActivities.FirstOrDefault(ua =>
            ua.ActivityId == id && ua.UserId == user.Id);

        if (registration == null) return NotFound();

        _context.UserActivities.Remove(registration);
        _context.SaveChanges();

        return Ok();
    }
}