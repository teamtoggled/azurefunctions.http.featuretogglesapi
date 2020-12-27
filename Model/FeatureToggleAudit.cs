using System;
using Newtonsoft.Json;

namespace UpdateFeatureToggles
{
    public class FeatureToggleAudit
    {
        [JsonProperty("id")]
        public Guid Id {get; set;}

        [JsonProperty("type")]
        public string Type {get; set;}

        [JsonProperty("configurationId")]
        public Guid ConfigurationId {get; set;}
        
        [JsonProperty("featureToggleId")]
        public Guid FeatureToggleId {get; set;}

        [JsonProperty("updatedDateTimeUtc")]
        public string UpdatedDateTimeUtc {get; set;}

        [JsonProperty("updatedBy")]
        public string UpdatedBy {get; set;}

        [JsonProperty("updatedToValue")]
        public string UpdatedToValue {get; set;}

        public FeatureToggleAudit()
        {
            this.Type = "toggled.featuretoggle.audit";
        }
    }
}
