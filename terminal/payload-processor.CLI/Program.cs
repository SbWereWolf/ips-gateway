using archivist;
using CommandLineInterface;
using PayloadProcessor;

namespace PayloadProcessor.CLI
{
    public class Program : BaseProgram
    {

        private const string SettingsKey = "payload-processor-settings-file";

        public Program(string settingsPathKey, string correlationId)
            : base(settingsPathKey, correlationId)
        {
        }

        [Log]
        public static void Main(string[] args)
        {
            var correlationId = ExtractCorrelationId(args);

            var app = new Program(SettingsKey, correlationId);

            var arguments = app.ProcureArguments(
                args,
                new ArgumentStorageFactory(correlationId)
                );
            app.Run(arguments);
        }

        [Log]
        public override string Run(IArgumentStorage extractor)
        {
            var jsonData = Console.ReadLine();

            string result = HandlePayload(jsonData ?? string.Empty);

            Console.WriteLine(result);

            return result;
        }

        [Log]
        private string HandlePayload(string jsonData)
        {
            var processor = new Processor(CorrelationId);
            var jsonResult = processor.HandlePayload(jsonData);

            return jsonResult;
        }
    }
}