namespace CommandLineInterface
{
    public class ArgumentStorage : IArgumentStorage
    {
        private readonly Picker Picker;
        public ArgumentStorage(string[]? args)
        {
            Picker = new Picker(args);
        }

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

        public override string ToString()
        {
            return $"{{ \"ArgumentStorage\": {{\"Picker\": {Picker} }} }}";
        }
    }
}