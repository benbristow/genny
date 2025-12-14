namespace Genny.Tests;

public static class TestHelpers
{
    public static IDisposable SuppressConsoleOutput()
    {
        var originalOut = Console.Out;
        var nullWriter = new StringWriter();
        Console.SetOut(nullWriter);
        
        return new ConsoleOutputSuppressor(originalOut);
    }
    
    private class ConsoleOutputSuppressor(TextWriter originalOut) : IDisposable
    {
        public void Dispose()
        {
            Console.SetOut(originalOut);
        }
    }
}
