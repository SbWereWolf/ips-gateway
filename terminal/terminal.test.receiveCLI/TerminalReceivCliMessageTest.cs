using CommandLineInterface;

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
            var correlationId =$"{BaseProgram.CorrelationIdKey}{Guid.NewGuid()}";

            Array.Resize(ref startupArguments, startupArguments.Length + 1);
            startupArguments[^1] = correlationId;

            Concierge.CLI.Program.Main(startupArguments);
            
            var conciergeResult = PurifyOutput();
            var input = new StringReader(conciergeResult);
            
            System.Console.SetIn(input);
            ResetOutput();

            var args = new string[] { correlationId };

            IpsMessageParser.CLI.Program.Main(args);
            
            var parserResult = PurifyOutput();
            input = new StringReader(parserResult);
            
            System.Console.SetIn(input);
            ResetOutput();

            PayloadProcessor.CLI.Program.Main(args);
        }

        protected override string TakeExpectedOutput()
        {
            return File.ReadAllText(@".\expected\payload-processor.CLI.stdout");
        }
    }
}