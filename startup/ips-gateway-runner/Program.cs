using Microsoft.Extensions.Configuration;
using ReadingInboxLibrary;
using CommandLineInterface;

namespace IpsGatewayRunner
{
    internal class Program
    {
        private const string GatesPathKey = "gates-conf-path";
        private const string GatesSection = "gates";
        private const string KindKey = "kind";
        private const string OptionsKey = "options";

        static void Main(string[] args)
        {
            var secretConfig = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            var fileWithGates = secretConfig[GatesPathKey];

            var specs = LoadGatesSpecification(fileWithGates);

            var factory = new ReadingInboxLibrary.InboxReaderFactory();
            var correlationId = BaseProgram.ExtractCorrelationId(args);

            var allReaders = new List<IReadingInbox>();
            foreach (var gate in specs)
            {
                var reader = factory.Make(gate.Medium, correlationId);
                reader.GetReadyForReading(gate.Options);

                allReaders.Add(reader);
            }

            Console.ReadKey();
        }

        private static List<GateSpecification> LoadGatesSpecification(string fileWithGates)
        {
            var gatesConfig = new ConfigurationBuilder()
                .AddJsonFile(fileWithGates)
                .Build();
            var gatesSpecs = gatesConfig.GetSection(GatesSection).GetChildren().ToList();

            var specification = new List<GateSpecification>();
            foreach (var gateSpecification in gatesSpecs)
            {
                var e = gateSpecification.GetChildren().GetEnumerator();
                var kind = "";
                var options = "";
                while (e.MoveNext())
                {
                    var key = e.Current.Key;
                    var value = e.Current.Value;

                    switch (key)
                    {
                        case KindKey:
                            kind = value;
                            break;
                        case OptionsKey:
                            options = value;
                            break;

                    }

                }

                specification.Add(new GateSpecification(kind, options));
            }

            return specification;
        }
    }
}