#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using System;
using System.Runtime.Serialization;

namespace DA_Assets.Shared
{
    public struct IDAError
    {
        public IDAError(int status = 0, string err = null, Exception ex = null)
        {
            this.Status = status;
            this.Message = err;
            this.Exception = ex;
        }

        public Exception Exception { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("status")] 
#endif
        public int Status { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("err")] 
#endif
        public string Message { get; set; }
    }
}