#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    public struct AuthResult
    {
#if JSONNET_EXISTS
        [JsonProperty("access_token")]
#endif
        public string AccessToken;
#if JSONNET_EXISTS
        [JsonProperty("expires_in")]
#endif
        public string ExpiresIn;
#if JSONNET_EXISTS
        [JsonProperty("refresh_token")]
#endif
        public string RefreshToken;
    }
}
