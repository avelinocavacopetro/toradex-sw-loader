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

        public async Task<List<SshKey>> GetFlatSshKeysAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<SshKey>();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var result = new List<SshKey>();

            if (root.TryGetProperty("keys", out var keysElement))
            {
                foreach (var property in keysElement.EnumerateObject())
                {
                    var id = property.Name;
                    var entry = property.Value;

                    var pubkey = entry.GetProperty("pubkey").GetString() ?? string.Empty;
                    var name = entry.GetProperty("meta").GetProperty("name").GetString() ?? string.Empty;

                    result.Add(new SshKey
                    {
                        Id = id,
                        Pubkey = pubkey,
                        Name = name
                    });
                }
            }

            return result;
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

        private string FormatDuration(int durationMinutes)
        {
            var ts = TimeSpan.FromMinutes(durationMinutes);

            if (ts.Hours > 0 && ts.Minutes > 0)
                return $"{ts.Hours}h{ts.Minutes}m";
            else if (ts.Hours > 0)
                return $"{ts.Hours}h";
            else
                return $"{ts.Minutes}m";
        }

        public async Task<HttpResponseMessage> SendCreateSession(string deviceUuid, int durationMinutes, string? publicKey = null)
        {
            string sshPubKey = publicKey?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(sshPubKey))
            {
                var baseUrl = $"https://app.torizon.io/api/v2beta/remote-access/device/{deviceUuid}/sessions";
                var getResponse = await _httpClient.GetAsync(baseUrl).ConfigureAwait(false);

                if (!getResponse.IsSuccessStatusCode)
                    return getResponse;

                var getContent = await getResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                RemoteSessionInfo? sessionInfo;
                try
                {
                    sessionInfo = JsonSerializer.Deserialize<RemoteSessionInfo>(getContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (JsonException ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent($"Erro ao desserializar resposta: {ex.Message}")
                    };
                }

                sshPubKey = sessionInfo?.Ssh?.RaServerSshPubKey?.Trim() ?? "";
            }

            var payload = new
            {
                publicKeys = new[] { sshPubKey },
                sessionDuration = FormatDuration(durationMinutes),
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var postUrl = $"https://app.torizon.io/api/v2beta/remote-access/device/{deviceUuid}/sessions";
            var postResponse = await _httpClient.PostAsync(postUrl, content).ConfigureAwait(false);

            return postResponse;
        }

        public async Task<HttpResponseMessage> SendCancelAsync(List<string> deviceUuid)
        {
            return await _httpClient.PatchAsJsonAsync("https://app.torizon.io/api/v2beta/updates", deviceUuid);
        }
    }
}
