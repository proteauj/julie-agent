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
        private readonly OpenAiChatService _openAiService;

        public ChatController(OpenAiChatService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponseDto>> Post([FromBody] ChatMessageDto dto)
        {
            var answer = await _openAiService.GetChatCompletion(dto.Message);
            return Ok(new ChatResponseDto { Response = answer });
        }
    }
}