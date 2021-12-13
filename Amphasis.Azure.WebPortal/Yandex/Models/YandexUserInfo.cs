using System.Collections.Generic;
using Newtonsoft.Json;

namespace Amphasis.Azure.WebPortal.Yandex.Models
{
    public class YandexUserInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("real_name")]
        public string RealName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("default_email")]
        public string DefaultEmail { get; set; }

        [JsonProperty("emails")]
        public IList<string> Emails { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("default_avatar_id")]
        public string DefaultAvatarId { get; set; }

        [JsonProperty("is_avatar_empty")]
        public bool IsAvatarEmpty { get; set; }
    }
}