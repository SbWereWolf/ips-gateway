using System.Xml;
using archivist;

namespace IpsMessageParser
{
    public class Parser : ILogable
    {
        private string XmlDocumentText;

        public string CorrelationId;

        public Parser(string xmlDocumentText, string correlationId)
        {
            XmlDocumentText = xmlDocumentText;
            CorrelationId = correlationId;
        }

        public string GetCorrelationId()
        {
            return CorrelationId;
        }

        [Log]
        public string Run()
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(XmlDocumentText);
            var jsonText = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmlDoc);

            return jsonText;
        }
    }
}