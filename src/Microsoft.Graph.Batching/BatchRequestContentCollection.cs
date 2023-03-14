using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Graph.Batching
{
    /// <summary>
    /// A collection of batch requests that are automatically managed.
    /// </summary>
    [Obsolete("Code is now part of Microsoft.Graph.Core, remove package SvRooij.Graph.Batching")]
    public class BatchRequestContentCollection
    {
        private readonly IBaseClient baseClient;
        private readonly List<BatchRequestContent> batchRequests;
        private readonly int splitAfterRequests;
        private BatchRequestContent currentRequest;
        private bool readOnly = false;

        /// <summary>
        /// Constructs a new <see cref="BatchRequestContentCollection"/>.
        /// </summary>
        /// <param name="baseClient">The <see cref="IBaseClient"/> for making requests</param>
        /// <param name="splitAfterRequests"></param>
        public BatchRequestContentCollection(IBaseClient baseClient, int splitAfterRequests = CoreConstants.BatchRequest.MaxNumberOfRequests)
        {
            this.baseClient = baseClient;
            this.splitAfterRequests = splitAfterRequests;
            batchRequests = new List<BatchRequestContent>();
            currentRequest = new BatchRequestContent(baseClient);
        }

        private void ValidateReadOnly()
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Batch request collection is already executed");
            }
        }

        private void SetupCurrentRequest()
        {
            ValidateReadOnly();
            if (currentRequest.BatchRequestSteps.Count >= splitAfterRequests)
            {
                batchRequests.Add(currentRequest);
                currentRequest = new BatchRequestContent(baseClient);
            }
        }

        /// <summary>
        /// Adds a <see cref="HttpRequestMessage"/> to batch request content.
        /// </summary>
        /// <param name="httpRequestMessage">A <see cref="HttpRequestMessage"/> to use to build a <see cref="BatchRequestStep"/> to add.</param>
        /// <returns>The requestId of the newly created <see cref="BatchRequestStep"/></returns>
        [Obsolete("Code is now part of Microsoft.Graph.Core, remove package SvRooij.Graph.Batching")]
        public string AddBatchRequestStep(HttpRequestMessage httpRequestMessage)
        {
            SetupCurrentRequest();
            return currentRequest.AddBatchRequestStep(httpRequestMessage);
        }

        /// <summary>
        /// Adds a <see cref="RequestInformation"/> to batch request content
        /// </summary>
        /// <param name="requestInformation">A <see cref="RequestInformation"/> to use to build a <see cref="BatchRequestStep"/> to add.</param>
        /// <returns>The requestId of the  newly created <see cref="BatchRequestStep"/></returns>
        [Obsolete("Code is now part of Microsoft.Graph.Core, remove package SvRooij.Graph.Batching")]
        public Task<string> AddBatchRequestStepAsync(RequestInformation requestInformation)
        {
            SetupCurrentRequest();
            return currentRequest.AddBatchRequestStepAsync(requestInformation);
        }

        /// <summary>
        /// Removes a <see cref="BatchRequestStep"/> from batch request content for the specified id.
        /// </summary>
        /// <param name="requestId">A unique batch request id to remove.</param>
        /// <returns>True or false based on removal or not removal of a <see cref="BatchRequestStep"/>.</returns>
        [Obsolete("Code is now part of Microsoft.Graph.Core, remove package SvRooij.Graph.Batching")]
        public bool RemoveBatchRequestStepWithId(string requestId)
        {
            ValidateReadOnly();
            var removed = currentRequest.RemoveBatchRequestStepWithId(requestId);
            if (!removed && batchRequests.Count > 0)
            {
                for (int i = 0; i < batchRequests.Count; i++)
                {
                    removed = batchRequests[i].RemoveBatchRequestStepWithId(requestId);
                    if (removed)
                    {
                        return true;
                    }
                }
            }
            return removed;
        }

        internal IEnumerable<BatchRequestContent> GetBatchRequestsForExecution()
        {
            readOnly = true;
            if (currentRequest.BatchRequestSteps.Count > 0)
            {
                batchRequests.Add(currentRequest);
            }

            return batchRequests;
        }
    }
}