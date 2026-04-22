using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Memora.Api.Models;
using Memora.Api.Services;
using Memora.Api.Data;

namespace Memora.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly MedicalLlmService _medLlm;
        private readonly OpenAiChatService _openai;
        private readonly MessageHistoryService _history;
        private readonly ReminderService _reminderService;
        private readonly DataContext _db;

        public ChatController(
            MedicalLlmService medLlm,
            OpenAiChatService openai,
            MessageHistoryService history,
            ReminderService reminder,
            DataContext db)
        {
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
                txt.Contains("ordonnance") ||
                (txt.Contains("pourquoi") && txt.Contains("médicament")) ||
                (txt.Contains("à quoi sert") && txt.Contains("médicament"));
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> Post([FromBody] ChatMessageDto dto)
        {
            var email = User.FindFirst("email")?.Value;
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            int userId = user.Id;

            // 🔹 Sauvegarde message utilisateur
            await _history.AddMessageAsync(userId, "user", dto.Message);

            string answer;

            if (IsMedical(dto.Message))
            {
                answer = await _medLlm.AskAsync(dto.Message);
            }
            else
            {
                var messages = await _history.GetLastMessagesAsync(userId, 6);

                var reminders = await _reminderService.GetUpcomingRemindersAsync(userId);

                var remindersText = reminders.Any()
                    ? string.Join(
                        "\n",
                        reminders.Select(r =>
                            $"- {r.ScheduledAt.ToLocalTime():g}: {r.Title}" +
                            (string.IsNullOrWhiteSpace(r.Description) ? "" : $" ({r.Description})")))
                    : "Aucun rappel à venir.";

                // 🔥 PROMPT enrichi (clé A4.1)
                var systemPrompt = $@"
Tu es Aline Écoute, un assistant intelligent pour personnes âgées.

Tu es :
- bienveillant
- simple
- rassurant
- clair

Tu aides avec :
- organisation
- rappels
- activités
- questions générales

Voici les rappels de l'utilisateur :
{remindersText}

Si la situation semble urgente, suggère de contacter un proche ou le 911.

Réponds toujours de façon courte, claire et chaleureuse.
";

                answer = await _openai.GetChatCompletion(
                    dto.Message,
                    messages,
                    systemPrompt
                );
            }

            // 🔹 Sauvegarde réponse AI
            await _history.AddMessageAsync(userId, "assistant", answer);

            return Ok(new ChatResponseDto { Response = answer });
        }
    }
}