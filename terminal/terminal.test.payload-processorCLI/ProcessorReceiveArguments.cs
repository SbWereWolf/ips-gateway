namespace Terminal.Test.PayloadProcessorCLI
{
    public class ProcessorReceiveArguments : Terminal.Test.CLI.SingleStepTestAbstract
    {
        [Test]
        public void Test()
        {
            this.Run();
        }

        protected override string[] ConfigureArguments()
        {
            return Array.Empty<string>();
        }

        protected override StringReader DefineInput()
        {
            var input = File.ReadAllText(@".\data\input.json");

            return new StringReader(input);
        }

        protected override void Act(string[] startupArguments)
        {
            PayloadProcessor.CLI.Program.Main(startupArguments);
        }

        protected override string TakeExpectedOutput()
        {
            return File.ReadAllText(@".\expected\payload-processor.CLI.stdout");
        }
    }
}