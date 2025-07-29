
using archivist;

namespace CommandLineInterface
{
    public class ArgumentStorageFactory : ILogable
    {
        private string CorrelationId;

        public ArgumentStorageFactory(string correlationId)
        {
            CorrelationId = correlationId;
        }

        public string GetCorrelationId()
        {
            return CorrelationId;
        }

        //correlationId;
        [Log]
        public IArgumentStorage MakeDummy()
        {
            var storage = new ArgumentStorage(
                        Array.Empty<string>(),
                        CorrelationId
                        );

            return storage;
        }

        [Log]
        public IArgumentStorage MakeReal(string[]? args)
        {
            var storage = new ArgumentStorage(args, CorrelationId);

            return storage;
        }
    }
}
