using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        
        string[] logFiles = Directory.GetFiles(directory, "*.log")
                                     .Select(Path.GetFileName)
                                     .Where(f => Regex.Match(f, @"\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2}.log").Success)
                                     .ToArray();

        DeleteOldFiles(logFiles, directory);

        string logFIlePath = Path.Combine(directory, $"{DateTime.Now:yyyy'-'MM'-'dd'T'HH'-'mm'-'ss}.log");

        Debug.Log($"[Happy.Sort] Using log file at path {logFIlePath}");

        return logFIlePath;
    }

    private static void DeleteOldFiles(string[] logFile, string directory)
    {
        try
        {
            Debug.Log($"Log file could is {logFile.Length}");
            if (logFile.Length > 5)
            {
                foreach (var file in logFile.OrderByDescending(l => l)
                                            .Skip(5))
                {
                    string combine = Path.Combine(directory,file);
                    Debug.Log($"[Happy.Sort] Deleting old log file {combine}");
                    File.Delete(combine);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Happy.Sort] Error thrown while trying to delete old log files");
            Debug.LogException(e);
            throw;
        }
        
    }

    public static Logger CreateLogger(IModConstants modConstant, string modPrefix)
    {
        return new LoggerConfiguration()
               .WriteTo.File(GetLogFIlePath(modConstant),
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

        if (logEvent.Exception is {} exception) Debug.LogException(exception);
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