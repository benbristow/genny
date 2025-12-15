using System.Text.RegularExpressions;
using Genny.Services;

namespace Genny.Tests.Services;

public partial class LoggerTests
{
    private const string TestMessage = "Test message";
    private const string VerboseMessage = "Verbose message";
    private const string FirstMessage = "First message";
    private const string SecondMessage = "Second message";

    [Fact]
    public void Log_WithMessage_OutputsMessageWithTimestamp()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.Log(TestMessage);
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldContain(TestMessage);
            TestMessagePattern().IsMatch(output).ShouldBeTrue();
        }
    }

    [Fact]
    public void Log_WithEmptyMessage_OutputsTimestampWithEmptyMessage()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.Log("");
            
            // Assert
            var output = stringWriter.ToString();
            TimestampPattern().IsMatch(output).ShouldBeTrue();
        }
    }

    [Fact]
    public void LogVerbose_WithVerboseTrue_OutputsMessage()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.LogVerbose(VerboseMessage, verbose: true);
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldContain(VerboseMessage);
            VerboseMessagePattern().IsMatch(output).ShouldBeTrue();
        }
    }

    [Fact]
    public void LogVerbose_WithVerboseFalse_DoesNotOutputMessage()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.LogVerbose(VerboseMessage, verbose: false);
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldBeEmpty();
        }
    }

    [Fact]
    public void LogEmpty_OutputsEmptyLine()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.LogEmpty();
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldBe(Environment.NewLine);
        }
    }

    [Fact]
    public void LogEmptyVerbose_WithVerboseTrue_OutputsEmptyLine()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.LogEmptyVerbose(verbose: true);
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldBe(Environment.NewLine);
        }
    }

    [Fact]
    public void LogEmptyVerbose_WithVerboseFalse_DoesNotOutput()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.LogEmptyVerbose(verbose: false);
            
            // Assert
            var output = stringWriter.ToString();
            output.ShouldBeEmpty();
        }
    }

    [Fact]
    public void Log_TimestampFormat_IsCorrect()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.Log(TestMessage);
            
            // Assert
            var output = stringWriter.ToString();
            var match = TimestampExtractionPattern().Match(output);
            match.Success.ShouldBeTrue();
            
            const string timeFormat = ":";
            var timestamp = match.Groups[1].Value;
            var parts = timestamp.Split(timeFormat[0]);
            parts.Length.ShouldBe(3);
            
            var hour = int.Parse(parts[0]);
            var minute = int.Parse(parts[1]);
            var second = int.Parse(parts[2]);
            
            hour.ShouldBeInRange(0, 23);
            minute.ShouldBeInRange(0, 59);
            second.ShouldBeInRange(0, 59);
        }
    }

    [Fact]
    public void Log_MultipleCalls_EachHasTimestamp()
    {
        // Arrange
        using (TestHelpers.SuppressConsoleOutput())
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);
            
            // Act
            Logger.Log(FirstMessage);
            Logger.Log(SecondMessage);
            
            // Assert
            var output = stringWriter.ToString();
            var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            lines.Length.ShouldBe(2);

            FirstMessagePattern().IsMatch(lines[0]).ShouldBeTrue();
            SecondMessagePattern().IsMatch(lines[1]).ShouldBeTrue();
        }
    }

    [GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\] Test message")]
    private static partial Regex TestMessagePattern();
    
    [GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\]")]
    private static partial Regex TimestampPattern();
    
    [GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\] Verbose message")]
    private static partial Regex VerboseMessagePattern();
    
    [GeneratedRegex(@"\[(\d{2}:\d{2}:\d{2})\]")]
    private static partial Regex TimestampExtractionPattern();
    
    [GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\] First message")]
    private static partial Regex FirstMessagePattern();
    
    [GeneratedRegex(@"\[\d{2}:\d{2}:\d{2}\] Second message")]
    private static partial Regex SecondMessagePattern();
}

