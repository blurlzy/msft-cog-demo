// ## reference links
// https://learn.microsoft.com/en-us/rest/api/cognitiveservices/accountmanagement/deployments/list?view=rest-cognitiveservices-accountmanagement-2023-05-01&tabs=HTTP

using Azure.Core;
using Azure.ResourceManager.CognitiveServices;
using Azure.ResourceManager;
using Azure.Identity;
using Azure;

// TESTING ONLT
Console.WriteLine("Initializing an ARM client......");

// authenticate your client
ArmClient client = new ArmClient(new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeVisualStudioCredential = true,
    ExcludeVisualStudioCodeCredential = true,
    ExcludeSharedTokenCacheCredential = true
}));

// this example assumes you already have this CognitiveServicesAccountResource created on azure
// for more information of creating CognitiveServicesAccountResource, please refer to the document of CognitiveServicesAccountResource
string subscriptionId = "<subscription-id>";
string resourceGroupName = "<Resource-Group>";
string accountName = "<AOAI-Service-Name>";
string deploymentName = "<Deployment-Name>";
ResourceIdentifier cognitiveServicesAccountResourceId = CognitiveServicesAccountResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, accountName);
CognitiveServicesAccountResource cognitiveServicesAccount = client.GetCognitiveServicesAccountResource(cognitiveServicesAccountResourceId);

// get the collection of this CognitiveServicesAccountDeploymentResource
CognitiveServicesAccountDeploymentCollection collection = cognitiveServicesAccount.GetCognitiveServicesAccountDeployments();

Console.WriteLine($"Loading deployments from {accountName}...");

// invoke the operation and iterate over the result
await foreach (CognitiveServicesAccountDeploymentResource item in collection.GetAllAsync())
{
    // the variable item is a resource, you could call other operations on this instance as well
    // but just for demo, we get its data from this resource instance
    CognitiveServicesAccountDeploymentData resourceData = item.Data;
    // for demo we just print out the id
    Console.WriteLine($"Succeeded on id: {resourceData.Id} / {resourceData.Name}");

    // test delete

}

// delete
// get the resource id
ResourceIdentifier cognitiveServicesAccountDeploymentResourceId = CognitiveServicesAccountDeploymentResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, accountName, deploymentName);
CognitiveServicesAccountDeploymentResource cognitiveServicesAccountDeployment = client.GetCognitiveServicesAccountDeploymentResource(cognitiveServicesAccountDeploymentResourceId);

// invoke the operation
await cognitiveServicesAccountDeployment.DeleteAsync(WaitUntil.Completed);

// delete
Console.ForegroundColor = ConsoleColor.Red;
Console.Write($"{ deploymentName } was deleted.");
