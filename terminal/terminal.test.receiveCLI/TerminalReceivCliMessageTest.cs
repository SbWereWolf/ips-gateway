namespace terminal.test.receiveCLI
{
    public class TerminalReceivCliMessageTest : Terminal.Test.CLI.SingleStepTestAbstract
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
            return new StringReader(@"file;.\data\");
        }

        protected override void Act(string[] startupArguments)
        {
            Concierge.CLI.Program.Main(startupArguments);
            
            var conciergeResult = PurifyOutput();            
            var input = new StringReader(conciergeResult);
            
            System.Console.SetIn(input);
            ResetOutput();

            IpsMessageParser.CLI.Program.Main(Array.Empty<string>());
            
            var parserResult = PurifyOutput();
            input = new StringReader(parserResult);
            
            System.Console.SetIn(input);
            ResetOutput();

            PayloadProcessor.CLI.Program.Main(Array.Empty<string>());
        }

        protected override string TakeExpectedOutput()
        {
            return File.ReadAllText(@".\expected\payload-processor.CLI.stdout");
        }
    }
}