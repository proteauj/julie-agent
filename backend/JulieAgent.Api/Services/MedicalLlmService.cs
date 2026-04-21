using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace JulieAgent.Api.Services
{
    public class MedicalLlmService
    {
        private readonly string _apiUrl;
        private readonly string _apiToken;
        private readonly HttpClient _http;

        public MedicalLlmService(IConfiguration config)
        {
            _apiUrl = config["MedicalLLM:Url"] ?? "";
            _apiToken = config["MedicalLLM:Token"] ?? "";
            _http = new HttpClient();
            if (!string.IsNullOrEmpty(_apiToken))
                _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
        }

        public async Task<string> AskAsync(string prompt)
        {
            var payload = new { inputs = prompt };
            var resp = await _http.PostAsJsonAsync(_apiUrl, payload);
            if (!resp.IsSuccessStatusCode)
                return "Désolé, le modèle médical n’a pas pu répondre.";

            // Format sortie : [{"generated_text": "..."}] ou {"generated_text": "..."}
            var json = await resp.Content.ReadAsStringAsync();
            try
            {
                // Essayons de parser la réponse HuggingFace :
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
                    return doc.RootElement[0].GetProperty("generated_text").GetString() ?? "";
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    return doc.RootElement.GetProperty("generated_text").GetString() ?? "";
            }
            catch { }
            return "Réponse non comprise du modèle médical.";
        }
    }
}