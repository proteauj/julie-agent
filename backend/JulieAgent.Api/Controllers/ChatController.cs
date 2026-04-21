using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JulieAgent.Api.Models;

namespace JulieAgent.Api.Controllers
{
    [ApiController]
    [Authorize] // 👈 protège le chat
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public ActionResult<ChatResponseDto> Post([FromBody] ChatMessageDto dto)
        {
            // TODO: remplacer ici par appel à un vrai LLM si voulu
            var answer = dto.Message switch
            {
                string m when m.Contains("bonjour", StringComparison.OrdinalIgnoreCase) => "Bonjour ! 👋",
                string m when m.Contains("qui es-tu", StringComparison.OrdinalIgnoreCase) => "Je suis Julie, ton agent virtuel !",
                _ => "Réponse automatique : " + dto.Message
            };
            return Ok(new ChatResponseDto { Response = answer });
        }
    }
}