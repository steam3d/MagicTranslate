using NLog;

namespace NlogConfiguration
{
    public class NlogConfig
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Configure(string path, LogLevel minLevel, LogLevel maxLevel)
        {
            var layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss} ${level:padding=-5:fixedlength=true} | ${callsite:padding=60:fixedlength=true:inner=Layout:alignmentOnTruncation=right} | ${message}";
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = path };
            logfile.ArchiveAboveSize = 20_971_520;
            logfile.MaxArchiveFiles = 1;
            logfile.Layout = layout;
            var traceconsole = new NLog.Targets.TraceTarget("traceconsole");
            traceconsole.Layout = layout;


            // Rules for mapping loggers to targets            
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRule(minLevel, maxLevel, traceconsole);
            config.AddRule(minLevel, maxLevel, logfile);

            // Apply config           
            LogManager.Configuration = config;
            Logger.Debug("Nlog configured");
        }

        public static void ChangeLogLevel(string level)
        {
            var config = LogManager.Configuration;
            var rule0 = config.LoggingRules[0];
            var rule1 = config.LoggingRules[1];
            rule0.SetLoggingLevels(LogLevelFromString(level), LogLevel.Fatal);
            rule1.SetLoggingLevels(LogLevelFromString(level), LogLevel.Fatal);
            LogManager.Configuration = config;
            Logger.Info("LogLevel changed to {0}", level);
        }

        public static LogLevel LogLevelFromString(string level)
        {
            switch (level)
            {
                case "Trace":
                    return LogLevel.Trace;
                case "Debug":
                    return LogLevel.Debug;
                case "Info":
                    return LogLevel.Info;
                case "Warn":
                    return LogLevel.Warn;
                case "Error":
                    return LogLevel.Error;
                case "Fatal":
                    return LogLevel.Fatal;
                case "Off":
                    return LogLevel.Off;
                default:
                    return LogLevel.Info;
            }
        }
    }
}
