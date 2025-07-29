using archivist;

namespace CommandLineInterface
{
    public interface IArgumentStorage : ILogable
    {
        public string Extract(
            int argumentIndex,
            string missingArgumentMessage
            );

        public string ToString();
    }
}
