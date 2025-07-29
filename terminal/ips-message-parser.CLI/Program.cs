using archivist;
using CommandLineInterface;
using IpsMessageParser;

namespace IpsMessageParser.CLI
{
    public class Program : BaseProgram
    {

        private const string SettingsKey = "ips-message-parser-settings-file";

        public Program(string settingsPathKey, string correlationId)
            : base(settingsPathKey, correlationId)
        {
        }

        [Log]
        public static void Main(string[] args)
        {
            var correlationId = ExtractCorrelationId(args);

            var app = archivist.
                LoggingDecorator<ICliProgram>.
                Create(
                new Program(SettingsKey, correlationId),
                correlationId
                );

            var arguments = app.ProcureArguments(
                args,
                new ArgumentStorageFactory(correlationId)
                );
            app.Run(arguments);
        }

        [Log]
        public override string Run(IArgumentStorage arguments)
        {
            var xmlMessage = Console.ReadLine();

            string jsonText = ParseIpsMessage(xmlMessage ?? string.Empty);

            Console.WriteLine(jsonText);

            return jsonText;
        }

        [Log]
        private string ParseIpsMessage(string xmlMessage)
        {
            var parser = new Parser(xmlMessage, CorrelationId);
            var jsonText = parser.Run();

            return jsonText;
        }
    }
}