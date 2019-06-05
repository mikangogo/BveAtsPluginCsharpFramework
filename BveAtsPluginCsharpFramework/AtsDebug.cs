using System.Diagnostics;

namespace AtsPlugin
{
    public static class AtsDebug
    {
        public enum LogLevelType
        {
            Information = 0,
            Warning,
            Error,
            Verbose,
        }

        public static LogLevelType OutputLogLevel { get; set; } = LogLevelType.Information;


        public static void Log(string message, LogLevelType logLevel)
        {
            var logLevelString = string.Empty;


            if ((int)OutputLogLevel > (int)logLevel)
            {
                return;
            }


            switch (logLevel)
            {
                case LogLevelType.Information:
                    logLevelString = "[I]";
                    break;

                case LogLevelType.Warning:
                    logLevelString = "[W]";
                    break;

                case LogLevelType.Error:
                    logLevelString = "[E]";
                    break;

                case LogLevelType.Verbose:
                    logLevelString = "[V]";
                    break;
            }


            Debug.WriteLine($"[{AtsModule.ModuleName}]{logLevelString}: {message}");
        }

        public static void LogVerbose(string message)
        {
            Log(message, LogLevelType.Verbose);
        }

        public static void LogError(string message)
        {
            Log(message, LogLevelType.Error);
        }

        public static void LogWarning(string message)
        {
            Log(message, LogLevelType.Warning);
        }

        public static void LogInfo(string message)
        {
            Log(message, LogLevelType.Information);
        }
    }
}
