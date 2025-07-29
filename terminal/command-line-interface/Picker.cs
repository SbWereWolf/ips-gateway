using archivist;

namespace CommandLineInterface
{
    public class Picker
    {
        private readonly string[] Args;

        public Picker(string[]? args)
        {
            Args = args ?? Array.Empty<string>();
        }

        public string Pick(int argumentIndex)
        {
            string argument = ExtractArgument(argumentIndex);

            return argument;
        }

        private string ExtractArgument(int argumentIndex)
        {
            var isValid = Args.Length > argumentIndex;

            var isArgumentDefined = false;
            if (isValid)
            {
                isArgumentDefined = Args[argumentIndex] != null;
            }

            var argument = string.Empty;
            if (isArgumentDefined)
            {
                argument = Args[argumentIndex];
            }

            return argument;
        }
        public override string ToString()
        {
            return $"{{ \"Picker\": {{\"Args\":[{new ArrayStringPrinter(Args)}]}} }}";
        }
    }
}