﻿using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Graph.Batching
{
    /// <summary>
    /// Handles batch request responses.
    /// </summary>
    public class BatchResponseContentCollection
    {
        private readonly List<BatchResponseContentObject> batchResponses;

        internal BatchResponseContentCollection()
        {
            batchResponses = new List<BatchResponseContentObject>();
        }

        internal void AddResponse(IEnumerable<string> keys, BatchResponseContent content)
        {
            batchResponses.Add(new BatchResponseContentObject(keys, content));
        }

        private BatchResponseContent? GetBatchResponseContaining(string requestId)
        {
            return batchResponses.FirstOrDefault(b => b.Keys.Contains(requestId))?.Response;
        }

        /// <summary>
        /// Gets a batch response as <see cref="HttpResponseMessage"/> for the specified batch request id.
        /// The returned <see cref="HttpResponseMessage"/> MUST be disposed since it implements an <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="requestId">A batch request id.</param>
        /// <returns>A <see cref="HttpResponseMessage"/> response object for a batch request.</returns>
        public Task<HttpResponseMessage> GetResponseByIdAsync(string requestId)
        {
            return GetBatchResponseContaining(requestId)?.GetResponseByIdAsync(requestId)
                ?? Task.FromResult<HttpResponseMessage>(null);
        }

        /// <summary>
        /// Gets a batch response as a requested type for the specified batch request id.
        /// </summary>
        /// <param name="requestId">A batch request id.</param>
        /// <param name="responseHandler">ResponseHandler to use for the response</param>
        /// <returns>A deserialized object of type T<see cref="HttpResponseMessage"/>.</returns>
        public Task<T> GetResponseByIdAsync<T>(string requestId, IResponseHandler responseHandler = null) where T : IParsable, new()
        {
            return GetBatchResponseContaining(requestId)?.GetResponseByIdAsync<T>(requestId, responseHandler)
                ?? Task.FromResult<T>(default);
        }

        /// <summary>
        /// Gets a batch response content as a stream
        /// </summary>
        /// <param name="requestId">A batch request id.</param>
        /// <returns>The response stream of the batch response object</returns>
        /// <remarks> Stream should be dispose once done with.</remarks>
        public Task<Stream> GetResponseStreamByIdAsync(string requestId)
        {
            var batch = GetBatchResponseContaining(requestId);
            if (batch is null)
            {
                return Task.FromResult<Stream>(default);
            }

            return batch.GetResponseStreamByIdAsync(requestId);
        }

        /// <summary>
        /// Gets all batch responses <see cref="Dictionary{String, HttpResponseMessage}"/>.
        /// All <see cref="HttpResponseMessage"/> in the dictionary MUST be disposed since they implement <see cref="IDisposable"/>.
        /// </summary>
        /// <returns>A Dictionary of id and <see cref="HttpResponseMessage"/> representing batch responses.</returns>
        /// <remarks>Not implemented, need help</remarks>
        public Task<Dictionary<string, HttpResponseMessage>> GetResponsesAsync()
        {
            throw new NotImplementedException("GetResponsesAsync() is not available in the BatchCollection");
        }
    }
}