using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Xml;

namespace PayloadProcessor
{
    public class Processor
    {
        public string HandlePayload(string payload)
        {
            var doc = JsonConvert.DeserializeXmlNode(payload);
            var result = doc?.InnerXml;

            return result ?? "";
        }
    }
}