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
    public static class UpdateToggle
    {
        public static BodyPostModel GetPostModel(HttpRequest req, ILogger log)
        {
            try
            {
                using (var reader = new StreamReader(req.Body))
                {
                    var body = reader.ReadToEnd();
                    
                    log.LogInformation($"Request body: {body}");

                    if(string.IsNullOrWhiteSpace(body))
                        return null;

                    return JsonConvert.DeserializeObject<BodyPostModel>(body);                
                }
            }
            catch (Exception)
            {
                return null;
            }            
        }

        [FunctionName("UpdateToggle")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "configurations/{configurationId}/featuretoggles/{featureToggleId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "toggled",
                collectionName: "part-configurationId",
                ConnectionStringSetting = "CosmosDbConnection",
                Id = "{featureToggleId}",
                PartitionKey = "{configurationId}")] FeatureToggle featureToggleIn,
            [CosmosDB(
                databaseName: "toggled",
                collectionName: "part-configurationId",
                ConnectionStringSetting = "CosmosDbConnection")]
                IAsyncCollector<FeatureToggle> featureTogglesOut,
            ILogger log)
        {
            log.LogInformation($"Running for {req.Path.ToString()}");

            var apiKey = req.Headers["x-api-key"].ToString();

            if(string.IsNullOrWhiteSpace(apiKey))
            {
                log.LogInformation("API key was missing. Returning BadRequest.");

                return new BadRequestObjectResult(new {
                    updated = false,
                    info = "An x-api-key header was not present."
                });
            }

            log.LogInformation($"API key: {apiKey}");

            var postModel = GetPostModel(req, log);

            if(postModel == null)
            {
                log.LogInformation("Request body was invalid. Returning BadRequest.");

                return new BadRequestObjectResult(new {
                    updated = false,
                    info = "The body was invalid, please ensure a json-encoded object is passed."
                });
            }

            log.LogInformation($"New value: {postModel.NewFeatureSwitchValue}");

            if(featureToggleIn != null)
            {
                log.LogInformation($"Found a feature toggle matching ID, named {featureToggleIn.Name}");
               
                featureToggleIn.LastUpdated.On = DateTime.UtcNow.ToString("O");
                featureToggleIn.State = postModel.NewFeatureSwitchValue;

                await featureTogglesOut.AddAsync(featureToggleIn);

                var result = new {
                    updated = true
                };

                return new OkObjectResult(result);
            }
            else
            {
                return new NotFoundObjectResult("Could not find a feature toggle for this configuration/id");
            }
        }
    }
}