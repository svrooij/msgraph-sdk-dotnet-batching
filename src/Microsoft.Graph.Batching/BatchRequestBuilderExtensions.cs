using Microsoft.Graph.Core.Requests;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Graph.Batching
{
    public static class BatchRequestBuilderExtensions
    {
        public static async Task<BatchResponseContentCollection> PostAsync(this BatchRequestBuilder batch, BatchRequestContentCollection batchRequestContentCollection, CancellationToken cancellationToken = default)
        {
            var collection = new BatchResponseContentCollection();

            var requests = batchRequestContentCollection.GetBatchRequestsForExecution();
            foreach (var request in requests)
            {
                var response = await batch.PostAsync(request, cancellationToken);
                collection.AddResponse(request.BatchRequestSteps.Keys, response);
            }

            return collection;
        }
    }
}