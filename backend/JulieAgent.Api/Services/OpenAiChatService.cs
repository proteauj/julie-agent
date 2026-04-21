using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Interfaces;

namespace JulieAgent.Api.Services
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
        

        public async Task<string> GetChatCompletion(string message)
        {
            bool isMedical = IsMedicalQuestion(message);
            HttpResponseMessage response;

            if (isMedical && _config["MedicalLLM:Url"] is { } medicalUrl && medicalUrl != "")
            {
                // Envoie la requête à l'API HuggingFace
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["MedicalLLM:Token"]}");
                var body = new { inputs = message };
                response = await client.PostAsJsonAsync(medicalUrl, body);
                var resp = await response.Content.ReadAsStringAsync();
                // Extrait la réponse selon format de ton modèle (adapter ici)
                return resp;
            }

            var api = new OpenAIAPI(_apiKey);

            var chat = api.Chat.CreateConversation();
            chat.Model = isMedical ? _medicalModel : _defaultModel;
            chat.AppendSystemMessage(isMedical ? _promptMedical : _promptCompagnon);
            chat.AppendUserInput(message);

            var chatResponse = await chat.GetResponseFromChatbotAsync();
            return chatResponse;
        }
    }
}