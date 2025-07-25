using CommandLineInterface;
using PayloadProcessor;

namespace PayloadProcessor.CLI
{
    public class Program : BaseProgram
    {

        private const string SettingsKey = "payload-processor-settings-file";

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

        public override string Run(IArgumentStorage extractor)
        {
            var jsonData = Console.ReadLine();

            string result = HandlePayload(jsonData ?? string.Empty);

            Console.WriteLine(result);

            return result;
        }

        private static string HandlePayload(string jsonData)
        {
            var processor = new Processor();
            var jsonResult = processor.HandlePayload(jsonData);

            return jsonResult;
        }
    }
}