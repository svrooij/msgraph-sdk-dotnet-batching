using System.Collections.Generic;

namespace Microsoft.Graph.Batching
{
    internal class BatchResponseContentObject
    {
        internal readonly IEnumerable<string> Keys;
        internal readonly BatchResponseContent Response;

        public BatchResponseContentObject(IEnumerable<string> keys, BatchResponseContent response)
        {
            Keys = keys;
            Response = response;
        }
    }
}