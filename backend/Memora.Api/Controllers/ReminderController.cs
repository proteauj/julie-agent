using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Memora.Api.Dtos;
using Memora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    [Authorize]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public ReminderController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminders = await _reminderService.GetAllRemindersAsync(userId.Value);
            return Ok(reminders.Select(ReminderService.ToDto));
        }

        [HttpGet("mine/upcoming")]
        public async Task<IActionResult> GetUpcoming([FromQuery] int hours = 24)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminders = await _reminderService.GetUpcomingRemindersAsync(userId.Value, hours);
            return Ok(reminders.Select(ReminderService.ToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminder = await _reminderService.GetReminderByIdAsync(id, userId.Value);
            if (reminder == null)
                return NotFound();

            return Ok(ReminderService.ToDto(reminder));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReminderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminder = await _reminderService.CreateReminderAsync(userId.Value, dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = reminder.Id },
                ReminderService.ToDto(reminder)
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReminderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminder = await _reminderService.UpdateReminderAsync(id, userId.Value, dto);
            if (reminder == null)
                return NotFound();

            return Ok(ReminderService.ToDto(reminder));
        }

        [HttpPost("{id:int}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var reminder = await _reminderService.MarkReminderDoneAsync(id, userId.Value);
            if (reminder == null)
                return NotFound();

            return Ok(ReminderService.ToDto(reminder));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var deleted = await _reminderService.DeleteReminderAsync(id, userId.Value);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("send-due")]
        public async Task<IActionResult> SendDueReminders()
        {
            await _reminderService.SendRemindersDueAsync();
            return Ok(new { message = "Reminders sent" });
        }

        private int? GetUserId()
        {
            var sub =
                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(sub, out var userId))
                return userId;

            return null;
        }
    }
}