namespace CommandLineInterface
{
    public interface IArgumentStorage
    {
        public string Extract(
            int argumentIndex,
            string missingArgumentMessage
            );

        public string ToString();
    }
}
