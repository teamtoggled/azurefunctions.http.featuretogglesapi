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
    public class BodyPostModel 
    {
        public bool NewFeatureSwitchValue {get; set;}
    }

    public static class UpdateToggle
    {
        public static BodyPostModel GetPostModel(HttpRequest req, ILogger log)
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = reader.ReadToEnd();
                
                log.LogInformation($"Request body: {body}");

                return JsonConvert.DeserializeObject<BodyPostModel>(body);                
            }
        }

        [FunctionName("UpdateToggle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "configurations/{configurationId}/featuretoggles/{featureToggleId}")] HttpRequest req,
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
            var apiKey = req.Headers["Api-Key"].ToString();
            log.LogInformation($"API key: {apiKey}");

            var postModel = GetPostModel(req, log);
            log.LogInformation($"New value: {postModel.NewFeatureSwitchValue}");

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
