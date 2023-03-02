using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Batching;
using Microsoft.Graph.Models;

var scopes = new[] { "User.Read" };

// Multi-tenant apps can use "common",
// single-tenant apps must use the tenant ID from the Azure portal
var tenantId = "df68aa03-48eb-4b09-9f3e-8aecc58e207c";

// Value from app registration
var clientId = "d3b14c33-10e5-469d-9300-8a6886e5ed04";

// using Azure.Identity;
// https://learn.microsoft.com/dotnet/api/azure.identity.interactivebrowsercredential
var interactiveCredential = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
{
    TenantId = tenantId,
    ClientId = clientId,
    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
    // MUST be http://localhost or http://localhost:PORT
    // See https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/System-Browser-on-.Net-Core
    RedirectUri = new Uri("http://localhost"),
});

var graphClient = new GraphServiceClient(interactiveCredential, scopes);

// Without batch collection
var batchRequestContent = new BatchRequestContent(graphClient);

var getRequest1 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
var getRequest2 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());

var response = await graphClient.Batch.PostAsync(batchRequestContent);

var user = await response.GetResponseByIdAsync<User>(getRequest1);
Console.WriteLine("Hi {0}", user.DisplayName);

var batchCollection = new BatchRequestContentCollection(graphClient);
var getCollection1 = await batchCollection.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
var getCollection2 = await batchCollection.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
var responseCollection = await graphClient.Batch.PostAsync(batchCollection);

var userFromCollection = await responseCollection.GetResponseByIdAsync<User>(getCollection1);

Console.WriteLine("Hi {0}", userFromCollection.DisplayName);
