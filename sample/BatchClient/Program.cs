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
TestAction action = TestAction.AddTasksWithBatch;
int numberOfTasksToCreate = 30;


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

if (action == TestAction.AddTasks || action == TestAction.AddTasksWithBatch)
{
    for (int i = 1; i < numberOfTasksToCreate + 1; i++)
    {
        tasks.Add(new TodoTask
        {
            Title = $"Test task {i}",
            Categories = new List<string> {"Important"}
        });
    }
} else
{
    Console.WriteLine("Loading tasks");
    tasks = await graphClient.GetAllTasksFromList(listId);
}

var stopwatch = new Stopwatch();
switch (action)
{
    case TestAction.AddTasks:
        Console.WriteLine("Start adding {0} tasks", tasks.Count);
        stopwatch.Start();
        await graphClient.AddTodoTasksToList(listId, tasks);

        break;


    case TestAction.AddTasksWithBatch:
        Console.WriteLine("Start adding {0} tasks with batch", tasks.Count);

        stopwatch.Start();
        await graphClient.AddTodoTasksToListWithBatch(listId, tasks);
        break;


    case TestAction.RemoveTasks:
        Console.WriteLine("Start removing {0} tasks", tasks.Count);
        stopwatch.Start();
        await graphClient.RemoveTasksFromList(listId, tasks);
        break;



    case TestAction.RemoveTasksWithBatch:
        Console.WriteLine("Start removing {0} tasks with batch", tasks.Count);
        stopwatch.Start();
        await graphClient.RemoveTasksFromListWithBatch(listId, tasks);

        break;
}

stopwatch.Stop();

Console.WriteLine("{0} completed in {1}ms", action, stopwatch.ElapsedMilliseconds);
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
