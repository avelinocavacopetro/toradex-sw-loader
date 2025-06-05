using System.Net.Http.Headers;
using System.Text.Json;
using ToradexSwLoader.Models;

namespace ToradexSwLoader.Services
{
    public class TorizonService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiClientId;
        private readonly string _apiSecret;
        private readonly string _tokenUrl;
        private string _accessToken;

        public TorizonService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiClientId = configuration["Torizon:ClientId"];
            _apiSecret = configuration["Torizon:Secret"];
            _tokenUrl = configuration["Torizon:TokenUrl"];
        }

        public async Task<bool> AuthenticateAsync()
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _apiClientId,
                ["client_secret"] = _apiSecret,
                ["grant_type"] = "client_credentials"
            });

            var response = await _httpClient.PostAsync(_tokenUrl, content);

            if (!response.IsSuccessStatusCode)
                return false;

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _accessToken = tokenResponse.AccessToken;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            return true;
        }

        public async Task<List<T>> GetItemsAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.TryGetProperty("values", out JsonElement valuesElement))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<T>>(valuesElement.GetRawText(), options);
            }

            return null;
        }

        public async Task ChangeSecretAndSave(string newSecret)
        {
            try
            {
                var filePath = "appsettings.json";

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("O arquivo appsettings.json não foi encontrado.");

                var json = await File.ReadAllTextAsync(filePath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement.Clone();

                var jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                if (jsonObj is null || !jsonObj.TryGetValue("Torizon", out var torizonSectionRaw))
                    throw new InvalidOperationException("Configuração 'Torizon' não encontrada.");

                var torizonJson = torizonSectionRaw.ToString();
                var torizonSection = JsonSerializer.Deserialize<Dictionary<string, string>>(torizonJson ?? "{}");

                if (torizonSection == null)
                    throw new InvalidOperationException("Estrutura 'Torizon' inválida.");

                torizonSection["Secret"] = newSecret;

                jsonObj["Torizon"] = torizonSection;

                var updatedJson = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, updatedJson);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao alterar o segredo: {ex.Message}");
                throw; 
            }
        }


    }
}
