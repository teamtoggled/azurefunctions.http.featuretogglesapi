using System;
using Newtonsoft.Json;

namespace UpdateFeatureToggles
{
    public class FeatureToggle 
    {
        [JsonProperty("id")]
        public Guid Id {get; set;}
        
        [JsonProperty("type")]
        public string Type {get; set;}
        
        [JsonProperty("configurationId")]
        public Guid ConfigurationId {get; set;}
        
        [JsonProperty("name")]
        public string Name {get; set;}
        
        [JsonProperty("state")]
        public bool State {get; set;}
        
        [JsonProperty("lastUpdated")]
        public LastUpdatedInfo LastUpdated {get; set;}
        
        [JsonProperty("signalRVaultUrl")]
        public string SignalRVaultUrl {get; set;}
    }
}
