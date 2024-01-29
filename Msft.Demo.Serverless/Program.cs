using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Azure.Identity;
using Azure.ResourceManager;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // inject services
        var credentialOpts = new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeSharedTokenCacheCredential = true
        };

        // arm client
        services.AddSingleton(m => new ArmClient(new DefaultAzureCredential(credentialOpts)));

    })
    .Build();

host.Run();
