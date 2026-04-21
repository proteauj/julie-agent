using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Interfaces;

namespace Memora.Api.Services
{
    public class OpenAiChatService
    {
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _defaultModel;
        private readonly string _medicalModel;
        private readonly string _promptCompagnon;
        private readonly string _promptMedical;

        public OpenAiChatService(IConfiguration config)
        {
            _config = config;
            _apiKey = _config["OpenAI:ApiKey"] ?? "";
            _defaultModel = _config["OpenAI:Model"] ?? "gpt-4o";
            _medicalModel = _config["OpenAI:MedicalModel"] ?? "gpt-3.5-turbo";
            _promptCompagnon = _config["OpenAI:SystemPromptCompagnon"] ?? "Bonjour !";
            _promptMedical = _config["OpenAI:SystemPromptMedical"] ?? "";
        }

        // SIMPLE INTENT DETECTION
        private bool IsMedicalQuestion(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var txt = text.ToLowerInvariant();
            return txt.Contains("effet secondaire") ||
                   txt.Contains("contre-indication") ||
                   txt.Contains("médicament") || txt.Contains("dose") ||
                   txt.Contains("prise") || txt.Contains("interraction") ||
                   txt.Contains("traitement") || txt.Contains("ordonnance") ||
                   txt.Contains("pourquoi") && txt.Contains("médicament") ||
                   txt.Contains("à quoi sert") && txt.Contains("médicament");
            // Tu peux enrichir la liste de mots-clefs si besoin
        }
        

        public async Task<string> GetChatCompletion(string message, List<Message>? history = null)
        {
            bool isMedical = IsMedicalQuestion(message);
            HttpResponseMessage response;

            if (isMedical && _config["MedicalLLM:Url"] is { } medicalUrl && medicalUrl != "")
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["MedicalLLM:Token"]}");
                    var body = new { inputs = message };
                    response = await client.PostAsJsonAsync(medicalUrl, body);
                    if (!response.IsSuccessStatusCode)
                        return "Sorry, the medical model could not answer.";
                    var json = await response.Content.ReadAsStringAsync();
                    // Parse correct HuggingFace response format
                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                            return doc.RootElement[0].GetProperty("generated_text").GetString() ?? "";
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                            return doc.RootElement.GetProperty("generated_text").GetString() ?? "";
                    }
                    catch { }
                    return "Medical model returned an unexpected format.";
                }
            }

            // OpenAI with memory/context
            var api = new OpenAIAPI(_apiKey);
            var chat = api.Chat.CreateConversation();
            chat.Model = isMedical ? _medicalModel : _defaultModel;
            chat.AppendSystemMessage(isMedical ? _promptMedical : _promptCompagnon);

            if (history != null)
            {
                foreach (var msg in history)
                {
                    if (msg.Role == "user")
                        chat.AppendUserInput(msg.Text);
                    else if (msg.Role == "assistant")
                        chat.AppendExampleChatbotOutput(msg.Text);
                }
            }
            chat.AppendUserInput(message);

            var chatResponse = await chat.GetResponseFromChatbotAsync();
            return chatResponse;
        }
    }
}