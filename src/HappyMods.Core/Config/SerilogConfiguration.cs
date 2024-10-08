using System;
using System.IO;
using HappyMods.Core.Unity;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using UnityEngine;
using Logger=Serilog.Core.Logger;

namespace HappyMods.Core.Config;

public static class SerilogConfiguration
{
    public static Logger CreateLogger(IModConstants modConstant, string modPrefix)
    {
        return new LoggerConfiguration()
                     .WriteTo.File(Path.Combine($"{modConstant.PersistentDataPath}", $"Happy.{modPrefix}.{DateTime.Now}.log"),
                         outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                     .WriteTo.UnitySink(modPrefix)
                     .CreateLogger();
    }
}

public class UnityLogSink(string modPrefix) : ILogEventSink
{
    private string AddPrefix(string logMessage) => $"[{modPrefix}] {logMessage}";
    
    public void Emit(LogEvent logEvent)
    {
        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose:
            case LogEventLevel.Debug:
            case LogEventLevel.Information:
                Debug.Log(AddPrefix(logEvent.RenderMessage()));
                break;
            case LogEventLevel.Warning:
                Debug.LogWarning(AddPrefix(logEvent.RenderMessage()));
                break;
            case LogEventLevel.Error:
            case LogEventLevel.Fatal:
                Debug.LogError(AddPrefix(logEvent.RenderMessage()));
                break;
        }

        if (logEvent.Exception is {} exception)
            Debug.LogException(exception);
    }
}

public static class UnityLogSinkExtensions
{
    public static LoggerConfiguration UnitySink(this LoggerSinkConfiguration loggerConfiguration,
                                                string modPrefix)
    {
        return loggerConfiguration.Sink(new UnityLogSink(modPrefix), LogEventLevel.Warning);
    }
}
