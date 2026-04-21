using Microsoft.AspNetCore.Mvc;
using Memora.Api.Services;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderService _reminderService;

        public ReminderController(ReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        // ... autres endpoints (Get, Add, etc.)

        [HttpPost("send-due")]  // POST /api/reminders/send-due
        public async Task<IActionResult> SendDueReminders()
        {
            await _reminderService.SendRemindersDueAsync();
            return Ok(new { message = "Reminders sent" });
        }
    }
}