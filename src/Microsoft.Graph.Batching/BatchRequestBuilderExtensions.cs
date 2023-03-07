using Microsoft.Graph.Core.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Graph.Batching
{
    /// <summary>
    /// BatchRequestBuilderExtensions makes sure you can use the BatchRequestBuilder.PostAsync for the new BatchRequestContentCollection
    /// </summary>
    public static class BatchRequestBuilderExtensions
    {
        /// <summary>
        /// Sends out the <see cref="BatchRequestContentCollection"/> using the POST method
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="batchRequestContentCollection">The <see cref="BatchRequestContentCollection"/> for the request</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to use for cancelling requests</param>
        /// <returns></returns>
        [Obsolete("Code is now part of Microsoft.Graph.Core, remove package SvRooij.Graph.Batching")]
        public static async Task<BatchResponseContentCollection> PostAsync(this BatchRequestBuilder builder, BatchRequestContentCollection batchRequestContentCollection, CancellationToken cancellationToken = default)
        {
            var collection = new BatchResponseContentCollection();

            var requests = batchRequestContentCollection.GetBatchRequestsForExecution();
            foreach (var request in requests)
            {
                var response = await builder.PostAsync(request, cancellationToken);
                collection.AddResponse(request.BatchRequestSteps.Keys, response);
            }

            return collection;
        }
    }
}