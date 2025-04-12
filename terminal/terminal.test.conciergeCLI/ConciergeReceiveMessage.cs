using Concierge.CLI;
using Terminal.Test.CLI;

namespace Terminal.Test.ConciergeCLI
{
    public class ConciergeReceiveMessage : SingleStepTestAbstract
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
        }

        protected override string TakeExpectedOutput()
        {
            return File.ReadAllText(@".\expected\courier.CLI.stdout");
        }
    }
}