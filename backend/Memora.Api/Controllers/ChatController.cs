using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Memora.Api.Services;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Authorize] // 👈 protège le chat
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {

        private readonly MedicalLlmService _medLlm;
        private readonly OpenAiChatService _openai;

        public ChatController(MedicalLlmService medLlm, OpenAiChatService openai, MessageHistoryService history) {
            _medLlm = medLlm;
            _openai = openai;
            _history = history;
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

        private readonly MessageHistoryService _history;

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> Post([FromBody] ChatMessageDto dto)
        {
            var userId = User.FindFirst("sub")?.Value ?? "anonymous";
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