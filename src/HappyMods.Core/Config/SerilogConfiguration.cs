using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HappyMods.Core.UnitySupport;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace HappyMods.Core.Config;

public static class SerilogConfiguration
{
    private static string GetLogFIlePath(IModConstants modConstant)
    {
        var directory = Path.Combine($"{modConstant.ModFolder}", "logs");
        
        string[] fileNames = Directory.GetFiles(directory, "*.log");
        Dictionary<DateTime, string> logFile = new();
        
        foreach (string fileNameWithType in fileNames)
        {
            var filename = fileNameWithType.Split('.')[0];

            if (DateTime.TryParse(filename, out DateTime dateTime))
            {
                logFile[dateTime] = fileNameWithType;
            }
        }

        DeleteOldFiles(logFile, directory);

        string logFIlePath = Path.Combine(directory, $"{DateTime.Now:yyyy'-'MM'-'dd'T'HH'-'mm'-'ss}.log");
        
        Debug.Log($"[Happy.Sort] Using log file at path {logFIlePath}");
        
        return logFIlePath;
    }
    
    private static void DeleteOldFiles(Dictionary<DateTime, string> logFile, string directory)
    {

        if (logFile.Count > 5)
        {
            foreach (var pair in logFile.OrderByDescending(pair => pair.Key)
                                        .Skip(5))
            {
                File.Delete(Path.Combine(directory, pair.Value));
            }
        }
    }

    public static Logger CreateLogger(IModConstants modConstant, string modPrefix)
    {
        return new LoggerConfiguration()
               .WriteTo.File(
                   GetLogFIlePath(modConstant),
                   outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
               .WriteTo.UnitySink(modPrefix)
               .Enrich.FromLogContext()
               .CreateLogger();
    }
}

public class UnityLogSink(string modPrefix) : ILogEventSink
{
    private string AddPrefix(string logMessage) => $"[Happy.{modPrefix}] {logMessage}";

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

public static class SerilogExtensions
{
    private static readonly ConcurrentDictionary<Type, ILogger> LogCache = new();
    
    public static ILogger GetLogger<T>(this ServiceProvider provider)
    {
        if (LogCache.TryGetValue(typeof(T), out var logger)) return logger;
        
        return LogCache[typeof(T)] = provider.GetRequiredService<ILogger>().ForContext<T>();
    }

    public static LoggerConfiguration UnitySink(this LoggerSinkConfiguration loggerConfiguration,
                                                string modPrefix)
    {
        return loggerConfiguration.Sink(new UnityLogSink(modPrefix), LogEventLevel.Warning);
    }
}