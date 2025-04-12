using NUnit.Framework;

namespace Terminal.Test.CLI;
abstract public class SingleStepTestAbstract
{
    private StringWriter? Writer;

    protected void Run()
    {
        var arguments = Arrange();

        Act(arguments);

        Assert();
    }

    private string[] Arrange()
    {
        var input = DefineInput();
        System.Console.SetIn(input);

        ResetOutput();

        string[] startupArguments = ConfigureArguments();

        return startupArguments;
    }

    protected void ResetOutput()
    {
        Writer = new StringWriter();
        System.Console.SetOut(Writer);
    }

    abstract protected void Act(string[] startupArguments);

    private void Assert()
    {
        var pureResult = PurifyOutput();
        var expected = TakeExpectedOutput();

        NUnit.Framework.Assert.That(pureResult, Is.EqualTo(expected));
    }

    public string PurifyOutput()
    {
        if (Writer == null)
        {
            throw new MissingFieldException(
                "Missing console output writer, please init it"
                );
        }

        var result = Writer.ToString();

        return result.TrimEnd(Environment.NewLine.ToCharArray());
    }

    abstract protected string[] ConfigureArguments();

    abstract protected StringReader DefineInput();

    abstract protected string TakeExpectedOutput();
}
