
using archivist;

namespace CommandLineInterface
{
    public class ArgumentStorageFactory
    {
        public IArgumentStorage MakeDummy(string correlationId)
        {
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(
                new ArgumentStorage(Array.Empty<string>()),
                    correlationId
                    );

            return storage;
        }

        public IArgumentStorage MakeReal(
            string[]? args,
            string correlationId
            )
        {
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(new ArgumentStorage(args), correlationId);

            return storage;
        }
    }
}
