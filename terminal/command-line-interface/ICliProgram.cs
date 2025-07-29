using archivist;
using Microsoft.Extensions.Configuration;

namespace CommandLineInterface
{
    public interface ICliProgram : ILogable
    {
        public IArgumentStorage ProcureArguments(
    string[] args,
    ArgumentStorageFactory storageFactory
    );
        public string Run(IArgumentStorage arguments);
        public IConfigurationRoot LoadProgramSettings();
        public string[]? GatherArgumentsFromInput(
            string? input,
            string cliArgumentsSeparator,
            string optionNumbers
            );
    }
}
