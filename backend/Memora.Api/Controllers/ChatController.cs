using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Memora.Api.Services;
using Memora.Api.Data;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Authorize] // 👈 protège le chat
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {

        private readonly MedicalLlmService _medLlm;
        private readonly OpenAiChatService _openai;
        private readonly MessageHistoryService _history;
        private readonly ReminderService _reminderService;
        private readonly DataContext _db;

        public ChatController(MedicalLlmService medLlm, OpenAiChatService openai, MessageHistoryService history, ReminderService reminder, DataContext db) {
            _medLlm = medLlm;
            _openai = openai;
            _history = history;
            _reminderService = reminder;
            _db = db;
        }

        private bool IsMedical(string question)
        {
            var txt = question.ToLowerInvariant();
            return txt.Contains("médicament") ||
                txt.Contains("effet secondaire") ||
                txt.Contains("contre-indication") ||
                txt.Contains("dose") ||
                txt.Contains("prise") ||
                txt.Contains("traitement") ||
                txt.Contains("ordonnance")   ||
                txt.Contains("pourquoi") && txt.Contains("médicament") ||
                txt.Contains("à quoi sert") && txt.Contains("médicament");
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> Post([FromBody] ChatMessageDto dto)
        {
            var email = User.FindFirst("email")?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();
            int userId = user.Id;

            await _history.AddMessageAsync(userId, "user", dto.Message);

            string answer;
            if (IsMedical(dto.Message))
            {
                // Option: compose a context if you want ("Here are pending reminders..." etc.)
                answer = await _medLlm.AskAsync(dto.Message);
            }
            else
            {
                var messages = await _history.GetLastMessagesAsync(userId, 6);
                // Option: fetch reminders and add to systemPrompt
                var reminders = await _reminderService.GetUpcomingRemindersAsync(userId);
                string contextReminders = "";
                if (reminders.Any())
                {
                    contextReminders = "Here are some reminders for the user:\n" +
                        string.Join("\n", reminders.Select(r => $"{r.Date:MMM dd HH:mm}: {r.Text}"));
                }
                answer = await _openai.GetChatCompletion(dto.Message, messages, contextReminders);
            }

            await _history.AddMessageAsync(userId, "assistant", answer);

            return Ok(new ChatResponseDto { Response = answer });
        }
    }
}