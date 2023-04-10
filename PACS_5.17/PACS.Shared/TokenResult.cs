using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PACS.Shared.Contact
{
    public class TokenResult
    {
        [JsonPropertyName("Success")]
        public bool Success => Errors == null || !Errors.Any();

        [JsonPropertyName("Errors")]
        public IEnumerable<string> Errors { get; set; }

        [JsonPropertyName("AccessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("TokenType")]
        public string TokenType { get; set; }

        public string UserId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
