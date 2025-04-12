using System.Xml;

namespace IpsMessageParser
{
    public class Parser
    {
        private string XmlDocumentText;

        public Parser(string xmlDocumentText)
        {
            XmlDocumentText = xmlDocumentText;
        }

        public string Run()
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(XmlDocumentText);
            var jsonText = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmlDoc);

            return jsonText;
        }
    }
}