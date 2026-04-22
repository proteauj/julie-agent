using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Memora.Api.Models;       // Pour Message
using Microsoft.Extensions.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;

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
            _promptCompagnon = _config["OpenAI:SystemPromptCompagnon"] ?? "You are a senior-friendly companion.";
            _promptMedical = _config["OpenAI:SystemPromptMedical"] ?? "You are a precise medical assistant for seniors.";
        }

        private bool IsMedicalQuestion(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var txt = text.ToLowerInvariant();
            return txt.Contains("effet secondaire")
                || txt.Contains("contre-indication")
                || txt.Contains("médicament")
                || txt.Contains("dose")
                || txt.Contains("prise")
                || txt.Contains("interraction")
                || txt.Contains("traitement")
                || txt.Contains("ordonnance")
                || (txt.Contains("pourquoi") && txt.Contains("médicament"))
                || (txt.Contains("à quoi sert") && txt.Contains("médicament"));
        }

        public async Task<string> GetChatCompletion(
            string message,
            List<Message>? history = null,
            string? systemPrompt = null)
        {
            bool isMedical = IsMedicalQuestion(message);

            // ROUTAGE médical
            if (isMedical && _config["MedicalLLM:Url"] is { } medicalUrl && medicalUrl != "")
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["MedicalLLM:Token"]}");
                var body = new { inputs = message };
                var response = await client.PostAsJsonAsync(medicalUrl, body);

                if (!response.IsSuccessStatusCode)
                    return "Sorry, the medical model could not answer.";

                var json = await response.Content.ReadAsStringAsync();
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

            // ROUTAGE OPENAI AVEC MEMOIRE ET SYSTEM PROMPT
            var api = new OpenAIAPI(_apiKey);
            var chat = api.Chat.CreateConversation();
            chat.Model = isMedical ? _medicalModel : _defaultModel;

            // Prompt système, rappels, etc.
            var basePrompt = systemPrompt ?? (isMedical ? _promptMedical : _promptCompagnon);

            var enrichedPrompt = basePrompt + @"
            Tu es Aline Écoute, un assistant intelligent pour personnes âgées.

            Tu es :
            - bienveillant
            - simple
            - rassurant

            Tu aides avec :
            - santé (sans poser de diagnostic)
            - rappels
            - organisation quotidienne

            Si une situation semble urgente, suggère de contacter un proche ou le 911.

            Réponds de façon courte, claire et chaleureuse.
            ";

            chat.AppendSystemMessage(enrichedPrompt);

            if (history != null)
            {
                foreach (var msg in history)
                {
                    if (msg.Role == "user")
                        chat.AppendUserInput(msg.Content);
                    else if (msg.Role == "assistant")
                        chat.AppendExampleChatbotOutput(msg.Content);
                }
            }
            chat.AppendUserInput(message);

            

            var chatResponse = await chat.GetResponseFromChatbotAsync();
            return chatResponse;
        }
    }
}