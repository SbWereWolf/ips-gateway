using Terminal.Test.CLI;
using IpsMessageParser.CLI;

namespace Terminal.Test.IpsMessageParserCLI
{
    public class ParserReceiveXml : Terminal.Test.CLI.SingleStepTestAbstract
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
            var input = File.ReadAllText(@".\data\input.xml");

            return new StringReader(input);
        }

        protected override void Act(string[] startupArguments)
        {
            IpsMessageParser.CLI.Program.Main(startupArguments);
        }

        protected override string TakeExpectedOutput()
        {
            return File.ReadAllText(@".\expected\ips-message-parser.CLI.stdout");
        }
    }
}