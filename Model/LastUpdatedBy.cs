using System;
using Newtonsoft.Json;

namespace UpdateFeatureToggles
{
    public class LastUpdatedBy 
    {
        [JsonProperty("id")]
        public Guid Id {get; set;}
        
        [JsonProperty("name")]
        public string Name {get; set;}
    }
}
