using archivist;

namespace CommandLineInterface
{
    public class EmptyValueChecker

    {
        private readonly string Value;
        public EmptyValueChecker(string value)
        {
            Value = value;
        }

        public bool CheckIsEmpty()
        {
            var result =  string.IsNullOrEmpty(Value);

            return result;
        }
    }
}