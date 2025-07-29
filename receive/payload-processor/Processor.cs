using archivist;
using Newtonsoft.Json;

namespace PayloadProcessor
{
    public class Processor : ILogable
    {
        public Processor(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public string CorrelationId { get; }

        public string GetCorrelationId()
        {
            return CorrelationId;
        }

        [Log]
        public string HandlePayload(string payload)
        {
            var doc = JsonConvert.DeserializeXmlNode(payload);
            var result = doc?.InnerXml;

            return result ?? "";
        }
    }
}