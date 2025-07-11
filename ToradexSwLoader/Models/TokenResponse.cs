﻿using System.Text.Json.Serialization;

namespace ToradexSwLoader.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
