using archivist;
using CommandLineInterface;

namespace Concierge.CLI
{
    public class Program : BaseProgram
    {
        private const int MediumIndex = 0;
        private const int OptionsIndex = 1;

        protected const string SettingsKey = "concierge-settings-file";

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

        public Program(string settingsPathKey, string correlationId)
            : base(settingsPathKey, correlationId)
        {
        }

        [Log]
        public override string Run(IArgumentStorage arguments)
        {
            var medium = arguments.Extract(
                MediumIndex,
                "Тип источника сообщений не задан"
                );
            var options = arguments.Extract(
                OptionsIndex,
                "Не определены настройки получения сообщений из источника"
                );

            var messageList = new List<string>();
            foreach (var message in ReadMessages(medium, options))
            {
                messageList.Add(message.ToString());
                Console.WriteLine(message);
            }

            return new archivist.ArrayStringPrinter(messageList.ToArray()).ToString();
        }

        [Log]
        public IEnumerable<string> ReadMessages(
            string medium,
            string options
            )
        {
            var reader = new ReadingInboxLibrary
                .InboxReaderFactory()
                .Make(medium, CorrelationId);
            reader.GetReadyForReading(options);

            foreach (var body in reader.LetReadTheMessages())
            {
                yield return body;
            }

        }
    }
}