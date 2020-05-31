using Newtonsoft.Json;

namespace UpdateFeatureToggles
{
    public class LastUpdatedInfo 
    {
        [JsonProperty("on")] 
        public string On {get; set;}
        
        [JsonProperty("by")]
        public LastUpdatedBy By {get; set;}
    }
}
