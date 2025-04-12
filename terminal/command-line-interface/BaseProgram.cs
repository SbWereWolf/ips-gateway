using Microsoft.Extensions.Configuration;

namespace CommandLineInterface
{
    public abstract class BaseProgram : ICliProgram
    {
        private const string GatherArgumentsFrom = "gather-arguments-from";
        private const string FromStartup = "startup";
        private const string FromInput = "input";
#pragma warning disable IDE0051 // Remove unused private members
        private const string DoNotGatherArguments = "none";
#pragma warning restore IDE0051 // Remove unused private members

        private const string CliArgumentsSeparator = "cli-arguments-separator";
        private const string CliArgumentsNumbers = "cli-argument-numbers";

        private readonly string SettingsFilePathKey = "program-settings-example.json";

        public BaseProgram(string settingsPathKey)
        {
            SettingsFilePathKey = settingsPathKey;
        }

        public IArgumentStorage ProcureArguments(
            string[] args,
            ArgumentStorageFactory storageFactory
            )
        {
            var settings = LoadProgramSettings();

            var programArguments = storageFactory.MakeDummy();
            var gatherArgumentsFrom = settings[GatherArgumentsFrom] ?? string.Empty;

            if (gatherArgumentsFrom == FromStartup)
            {
                programArguments = storageFactory.MakeReal(args);
            }

            if (gatherArgumentsFrom == FromInput)
            {
                var cliArgumentsSeparator = settings[CliArgumentsSeparator];
                var optionNumbers = settings[CliArgumentsNumbers];
                var input = Console.ReadLine();

                var arguments = GatherArgumentsFromInput(
                    input,
                    cliArgumentsSeparator,
                    optionNumbers
                    );

                programArguments = storageFactory.MakeReal(arguments);
            }

            return programArguments;
        }

        public abstract string Run(IArgumentStorage arguments);

        public IConfigurationRoot LoadProgramSettings()
        {
            var secretConfig = new ConfigurationBuilder()
                .AddUserSecrets<BaseProgram>()
                .Build();
            var fileWithSettings = secretConfig[SettingsFilePathKey] ?? "";

            var config = new ConfigurationBuilder().Build();
            if (!string.IsNullOrEmpty(fileWithSettings))
            {
                config = new ConfigurationBuilder()
                    .AddJsonFile(fileWithSettings)
                    .Build();
            }

            return config;
        }

        public string[]? GatherArgumentsFromInput(
            string? input,
            string cliArgumentsSeparator,
            string optionNumbers
            )
        {
            _ = int.TryParse(optionNumbers, out int ArgumentNumbers);
            var arguments =
                input?.Split(cliArgumentsSeparator, ArgumentNumbers) ??
                Array.Empty<string>();

            return arguments;
        }
    }
}