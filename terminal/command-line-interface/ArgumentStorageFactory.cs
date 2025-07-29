
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
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(
                    new ArgumentStorage(
                        Array.Empty<string>(),
                        CorrelationId
                        ),
                    CorrelationId
                    );

            return storage;
        }

        [Log]
        public IArgumentStorage MakeReal(string[]? args)
        {
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(
                new ArgumentStorage(args, CorrelationId),
                CorrelationId
                );

            return storage;
        }
    }
}
