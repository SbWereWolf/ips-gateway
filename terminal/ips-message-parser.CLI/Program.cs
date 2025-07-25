using CommandLineInterface;
using IpsMessageParser;

namespace IpsMessageParser.CLI
{
    public class Program : BaseProgram
    {

        private const string SettingsKey = "ips-message-parser-settings-file";

        public Program(string settingsPathKey) : base(settingsPathKey)
        {
        }

        public static void Main(string[] args)
        {
            var correlationId = GetCorrelationId(args);

            var app = archivist.
                LoggingDecorator<ICliProgram>.
                Create(new Program(SettingsKey), correlationId);

            app.SetCorrelationId(correlationId);

            var arguments = app.ProcureArguments(
                args,
                new ArgumentStorageFactory()
                );
            app.Run(arguments);
        }

        public override string Run(IArgumentStorage arguments)
        {
            var xmlMessage = Console.ReadLine();

            string jsonText = ParseIpsMessage(xmlMessage ?? string.Empty);

            Console.WriteLine(jsonText);

            return jsonText;
        }

        private static string ParseIpsMessage(string xmlMessage)
        {
            var parser = new Parser(xmlMessage);
            var jsonText = parser.Run();

            return jsonText;
        }
    }
}