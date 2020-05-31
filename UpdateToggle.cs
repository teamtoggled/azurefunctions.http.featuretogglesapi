using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

    public class LastUpdatedBy 
    {
        [JsonProperty("id")]
        public Guid Id {get; set;}
        
        [JsonProperty("name")]
        public string Name {get; set;}
    }
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

    public static class UpdateToggle
    {
        [FunctionName("UpdateToggle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "configurations/{configurationId}/featuretoggles/{featureToggleId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "toggled",
                collectionName: "beta-featuretoggles",
                ConnectionStringSetting = "CosmosDbConnection",
                Id = "{featureToggleId}",
                PartitionKey = "{configurationId}")] FeatureToggle featureToggleIn,
            [CosmosDB(
                databaseName: "toggled",
                collectionName: "beta-featuretoggles",
                ConnectionStringSetting = "CosmosDbConnection")]
                IAsyncCollector<FeatureToggle> featureTogglesOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            if(featureToggleIn != null)
            {
                log.LogInformation($"Got a toggle named {featureToggleIn.Name}");

                featureToggleIn.LastUpdated.On = DateTime.UtcNow.ToString("O");
                await featureTogglesOut.AddAsync(featureToggleIn);

                return new OkObjectResult("We've updated the LastUpdated.On of toggle.");
            }
            else
            {
                return new NotFoundObjectResult("Could not find a feature toggle for this configuration/id");
            }
        }
    }
}
