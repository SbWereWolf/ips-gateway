using archivist;

namespace CommandLineInterface
{
    public class ArgumentStorage : IArgumentStorage
    {
        private readonly Picker Picker;

        private string CorrelationId;

        public ArgumentStorage(string[]? args, string correlationId)
        {
            Picker = new Picker(args);
            CorrelationId = correlationId;
        }

        [Log]
        public string Extract(
            int argumentIndex,
            string missingArgumentMessage
            )
        {
            string argumentValue = Picker.Pick(argumentIndex);

            var tester = new EmptyValueChecker(argumentValue);
            var isArgumentEmpty = tester.CheckIsEmpty();

            tester = new EmptyValueChecker(missingArgumentMessage);
            var letThrowOnEmpty = !tester.CheckIsEmpty();

            if (isArgumentEmpty && letThrowOnEmpty)
            {
                throw new MissingFieldException(missingArgumentMessage);
            }

            return argumentValue;
        }

        public string GetCorrelationId()
        {
            return CorrelationId;
        }

        public override string ToString()
        {
            return $"{{ \"ArgumentStorage\": {{\"Picker\": {Picker} }} }}";
        }
    }
}