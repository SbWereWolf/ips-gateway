
using archivist;

namespace CommandLineInterface
{
    public class ArgumentStorageFactory
    {
        public IArgumentStorage MakeDummy()
        {
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(new ArgumentStorage(Array.Empty<string>()));

            return storage;
        }

        public IArgumentStorage MakeReal(string[]? args)
        {
            var storage = archivist.
                LoggingDecorator<IArgumentStorage>.
                Create(new ArgumentStorage(args));

            return storage;
        }
    }
}
