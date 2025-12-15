namespace Genny.Services;

/// <summary>
/// Helper class for logging with timestamps.
/// </summary>
public static class Logger
{
    /// <summary>
    /// Logs a message with a timestamp.
    /// </summary>
    public static void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"[{timestamp}] {message}");
    }

    /// <summary>
    /// Logs a message with a timestamp only if verbose mode is enabled.
    /// </summary>
    public static void LogVerbose(string message, bool verbose)
    {
        if (verbose)
        {
            Log(message);
        }
    }

    /// <summary>
    /// Logs an empty line (no timestamp).
    /// </summary>
    public static void LogEmpty()
    {
        Console.WriteLine();
    }

    /// <summary>
    /// Logs an empty line only if verbose mode is enabled.
    /// </summary>
    public static void LogEmptyVerbose(bool verbose)
    {
        if (verbose)
        {
            LogEmpty();
        }
    }
}
