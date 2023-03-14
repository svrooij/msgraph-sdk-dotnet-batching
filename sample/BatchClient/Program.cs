using Azure.Identity;
using BatchClient;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Diagnostics;

var scopes = new[] { "User.Read", "Tasks.ReadWrite" };

// Multi-tenant apps can use "common",
// single-tenant apps must use the tenant ID from the Azure portal (replace with your own)
var tenantId = "organizations";

// Value from app registration (replace with your own)
var clientId = Environment.GetEnvironmentVariable("ClientID") ?? "4cb89509-b479-480a-ad26-44c3a32a65e2";

// Change accordingly
string? listId = Environment.GetEnvironmentVariable("ListID") ?? null; // Or set list ID if you know it.

int numberOfTasksToCreate = 20;

Console.ReadLine();
GraphClientExtensions.WriteHeader();
Console.WriteLine("Press enter to start login");
Console.ReadLine();

// ----------------------------------------------- Code from here -----------------------------------------------
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

// To force authentication early, request profile
var profile = await graphClient.Me.GetAsync();

Console.WriteLine("Welcome {0}, press enter to start adding {1} tasks", profile.DisplayName, numberOfTasksToCreate);
Console.ReadLine();

// Create a new list for testing, deleting will remove all items from the list if you're not carefull!
if(string.IsNullOrEmpty(listId))
{
    var list = await graphClient.Me.Todo.Lists.PostAsync(new TodoTaskList
    {
        DisplayName = "Batch test list"
    });
    listId = list.Id;
    Console.WriteLine("Be sure to set the 'ListID' envirionment variable in LaunchSettings.json to:");
    Console.WriteLine(listId);
}

List<TodoTask> tasks = new List<TodoTask>();
for (int i = 1; i < numberOfTasksToCreate + 1; i++)
{
    tasks.Add(new TodoTask
    {
        Title = $"Test task {i}",
    });
}

var stopwatch = new Stopwatch();
stopwatch.Start();
await graphClient.AddTodoTasksToList(listId, tasks);
stopwatch.Stop();

Console.WriteLine("{0} completed in {1}ms", "AddTodoTasksToList", stopwatch.ElapsedMilliseconds);
Console.WriteLine("Press enter to continue");
Console.ReadLine();

tasks = await graphClient.GetAllTasksFromList(listId);
stopwatch.Restart();
await graphClient.RemoveTasksFromList(listId, tasks);
stopwatch.Stop();

Console.WriteLine("{0} completed in {1}ms", "RemoveTasksFromList", stopwatch.ElapsedMilliseconds);
Console.WriteLine("Press enter to continue");
Console.ReadLine();
tasks.Clear();
for (int i = 1; i < numberOfTasksToCreate + 1; i++)
{
    tasks.Add(new TodoTask
    {
        Title = $"Test task {i}",
    });
}
stopwatch.Restart();
await graphClient.AddTodoTasksToListWithBatch(listId, tasks);
stopwatch.Stop();

Console.WriteLine("{0} completed in {1}ms", "AddTodoTasksToListWithBatch", stopwatch.ElapsedMilliseconds);
Console.WriteLine("Press enter to continue");
Console.ReadLine();

tasks = await graphClient.GetAllTasksFromList(listId);
stopwatch.Restart();
await graphClient.RemoveTasksFromListWithBatch(listId, tasks);
stopwatch.Stop();

Console.WriteLine("{0} completed in {1}ms", "RemoveTasksFromListWithBatch", stopwatch.ElapsedMilliseconds);
Console.WriteLine("Press enter to continue");
Console.ReadLine();


Console.WriteLine("Tranks for watching, be sure to leave a star on this sample repository!");
Console.ReadLine();


//// Using a regular batch
//var batchRequestContent = new BatchRequestContent(graphClient);

//var getRequest1 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
//var getRequest2 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());

//var response = await graphClient.Batch.PostAsync(batchRequestContent);

//var user = await response.GetResponseByIdAsync<User>(getRequest1);
//Console.WriteLine("Hi {0}", user.DisplayName);

//var batchCollection = new BatchRequestContentCollection(graphClient);
//var getCollection1 = await batchCollection.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
//var getCollection2 = await batchCollection.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
//var responseCollection = await graphClient.Batch.PostAsync(batchCollection);

//var userFromCollection = await responseCollection.GetResponseByIdAsync<User>(getCollection1);

//Console.WriteLine("Hi {0}", userFromCollection.DisplayName);

enum TestAction
{
    AddTasks,
    AddTasksWithBatch,
    RemoveTasks,
    RemoveTasksWithBatch
}
