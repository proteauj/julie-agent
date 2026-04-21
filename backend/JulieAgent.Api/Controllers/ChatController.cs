using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JulieAgent.Api.Models;
using JulieAgent.Api.Services;

namespace JulieAgent.Api.Controllers
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
            if(IsMedical(dto.Message))
                answer = await _medLlm.AskAsync(dto.Message); // Peut supporter le contexte, ou simplement last msg
            else {
                var messages = await _history.GetLastMessagesAsync(userId);
                answer = await _openai.GetChatCompletion(dto.Message, messages);
            }

            await _history.AddMessageAsync(userId, "assistant", answer);

            return Ok(new ChatResponseDto { Response = answer });
        }
    }
}