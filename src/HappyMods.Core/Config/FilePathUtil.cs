using System;

namespace HappyMods.Core.Config;

public class Path
{
    public static string Combine(params string[] paths) =>
        new Uri(System.IO.Path.Combine(paths)).LocalPath;
    public static string? GetDirectoryName(string filePath) =>
        System.IO.Path.GetDirectoryName(filePath);
}

public class ContextLogger<T>(Logger logger)
{
    public readonly ILogger Logger = logger.ForContext<T>();
}
