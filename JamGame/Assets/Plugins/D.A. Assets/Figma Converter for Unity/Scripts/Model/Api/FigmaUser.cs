#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using System;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct FigmaUser
    {
#if JSONNET_EXISTS
        [JsonProperty("id")]
#endif
        public string Id { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("handle")]
#endif
        public string Name { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("email")]
#endif
        public string Email { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("img_url")]
#endif
        public string ImgUrl { get; set; }
    }
}