using System.Net;
using System.Text.Json;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.CognitiveServices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Msft.Demo.Serverless
{
    public class CogServiceFunc
    {
        private readonly ILogger _logger;
        private readonly ArmClient _client;

        public CogServiceFunc(ArmClient client, ILoggerFactory loggerFactory)
        {
            _client = client;
            _logger = loggerFactory.CreateLogger<CogServiceFunc>();
        }

        [Function("DeleteOpenAIDeployment")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // retreive request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            _logger.LogInformation($"Request body: {requestBody}");

            // deserialize 

            AoaiResource? model = JsonSerializer.Deserialize<AoaiResource>(requestBody, new JsonSerializerOptions
                                                                                        {
                                                                                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                                        });

            if (model == null)
            {
                // todo:
                _logger.LogError($"Invalid request.");

                var res = req.CreateResponse(HttpStatusCode.BadRequest);
                res.WriteString($"Invalid request");
                return res;
            }

            _logger.LogInformation($"Subcription id: {model.SubscriptionId}");
            _logger.LogInformation($"Resource group: {model.ResourceGroupName}");
            _logger.LogInformation($"Resource: {model.AccountName}");

            try
            {

                _logger.LogInformation($"Deleting AOAI deployment: {model.AccountName} / {model.DeploymentName}");

                await DeleteDeploymentAsync(model.SubscriptionId, model.ResourceGroupName, model.AccountName, model.DeploymentName);

                //  todo
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString($"Deployment model: {model.DeploymentName} was removed!");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);

                var errResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                errResponse.WriteString($"Error: {ex.Message}");
                return errResponse;
            }

        }


        private async Task DeleteDeploymentAsync(string subscriptionId, string resourceGroupName, string accountName, string deploymentName)
        {
            // delete
            // get the resource id
            ResourceIdentifier cognitiveServicesAccountDeploymentResourceId = CognitiveServicesAccountDeploymentResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, accountName, deploymentName);
            CognitiveServicesAccountDeploymentResource cognitiveServicesAccountDeployment = _client.GetCognitiveServicesAccountDeploymentResource(cognitiveServicesAccountDeploymentResourceId);

            // invoke the operation
            await cognitiveServicesAccountDeployment.DeleteAsync(WaitUntil.Completed);

        }

        private CognitiveServicesAccountDeploymentCollection GetDeploymentsAsync(string subscriptionId, string resourceGroupName, string accountName)
        {
            // retreive
            ResourceIdentifier cognitiveServicesAccountResourceId = CognitiveServicesAccountResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, accountName);

            // get resource by its resource id
            CognitiveServicesAccountResource cognitiveServicesAccount = _client.GetCognitiveServicesAccountResource(cognitiveServicesAccountResourceId);

            // get the collection of this CognitiveServicesAccountDeploymentResource
            CognitiveServicesAccountDeploymentCollection collection = cognitiveServicesAccount.GetCognitiveServicesAccountDeployments();
            
            return collection;
        }
    }

    public record AoaiResource
    {
        public string SubscriptionId { get; init; }
        public string ResourceGroupName { get; init; }
        public string AccountName { get; init; } // resource name
        public string DeploymentName { get; init; }
    }
}
