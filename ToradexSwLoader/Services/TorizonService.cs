using System.Net;
using System.Net.Http.Headers;
using System.Text;
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
            if(tokenResponse == null) return false;
            _accessToken = tokenResponse.AccessToken;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            return true;
        }

        public async Task<List<T>?> GetItemsAsync<T>(string url)
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
        public async Task<T?> GetItemAsync<T>(string url)
        {
            Console.WriteLine($"[HTTP] GET {url}");

            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[HTTP] Status: {response.StatusCode}");
            Console.WriteLine($"[HTTP] Body: {content}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("[HTTP] Request falhou.");
                return default;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(content, options);
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

        public async Task<string?> GetDeviceStatusAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("deviceStatus", out var statusElement))
                return statusElement.GetString();

            return null;
        }

        public async Task<HttpResponseMessage> SendUpdateAsync(DeviceDTO deviceDto)
        {
            return await _httpClient.PostAsJsonAsync("https://app.torizon.io/api/v2beta/updates", deviceDto);
        }

        public async Task<HttpResponseMessage> SendCreateSession(string deviceUuid, int durationMinutes)
        {
            var getUrl = $"https://app.torizon.io/api/v2beta/remote-access/device/{deviceUuid}/sessions";
            var getResponse = await _httpClient.GetAsync(getUrl);

            if (!getResponse.IsSuccessStatusCode)
            {
                return getResponse;
            }

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var sessionInfo = JsonSerializer.Deserialize<RemoteSessionInfo>(getContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var sshPubKey = sessionInfo?.Ssh?.RaServerSshPubKey;

            if (string.IsNullOrWhiteSpace(sshPubKey))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Chave pública SSH inválida ou não encontrada.")
                };
            }

            var postUrl = $"https://app.torizon.io/api/v2beta/remote-access/device/{deviceUuid}/sessions";

            var payload = new
            {
                publicKeys = new[] { sshPubKey },
                sessionDuration = TimeSpan.FromMinutes(durationMinutes).ToString(@"hh\:mm\:ss")
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var postResponse = await _httpClient.PostAsync(postUrl, content);

            return postResponse;
        }


        public async Task<HttpResponseMessage> SendCancelAsync(List<string> deviceUuid)
        {
            return await _httpClient.PatchAsJsonAsync("https://app.torizon.io/api/v2beta/updates", deviceUuid);
        }
    }
}
