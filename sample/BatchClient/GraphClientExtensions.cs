using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BatchClient
{
    internal static class GraphClientExtensions
    {
        internal static async Task AddTodoTasksToList(this GraphServiceClient graphClient, string? listId, List<TodoTask> tasks)
        {
            foreach (var task in tasks)
            {
                await graphClient.Me.Todo.Lists[listId].Tasks.PostAsync(task);
            }
        }

        internal static async Task AddTodoTasksToListWithBatch(this GraphServiceClient graphClient, string? listId, List<TodoTask> tasks)
        {
            var batchCollection = new BatchRequestContentCollection(graphClient);
            foreach (var task in tasks)
            {
                await batchCollection.AddBatchRequestStepAsync(graphClient.Me.Todo.Lists[listId].Tasks.ToPostRequestInformation(task));
            }
            await graphClient.Batch.PostAsync(batchCollection);
        }

        internal async static Task<List<TodoTask>> GetAllTasksFromList(this GraphServiceClient graphClient, string? listId)
        {
            try
            {
                var result = await graphClient.Me.Todo.Lists[listId].Tasks.GetAsync(request =>
                {
                    request.QueryParameters.Top = 100;
                    // Only select ID and Title for faster response.
                    //request.QueryParameters.Select = new[] { "id", "title" };

                });

                return result.Value;
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }

        internal async static Task RemoveTasksFromList(this GraphServiceClient graphClient, string? listId, List<TodoTask> tasks)
        {
            foreach (var task in tasks)
            {
                await graphClient.Me.Todo.Lists[listId].Tasks[task.Id].DeleteAsync();
            }
        }

        internal async static Task RemoveTasksFromListWithBatch(this GraphServiceClient graphClient, string? listId, List<TodoTask> tasks)
        {
            var batchCollectionForRemove = new BatchRequestContentCollection(graphClient);
            foreach (var task in tasks)
            {
                await batchCollectionForRemove.AddBatchRequestStepAsync(graphClient.Me.Todo.Lists[listId].Tasks[task.Id].ToDeleteRequestInformation());
            }
            await graphClient.Batch.PostAsync(batchCollectionForRemove);
        }
    }
}
