using System;

namespace HappyMods.Core.Config;

/// <summary>
/// A wrapper for <see cref="System.IO.Path"/> to ensure paths are returned in a system agnostig way
/// </summary>
public class Path
{
    public static string Combine(params string[] paths) =>
        new Uri(System.IO.Path.Combine(paths)).LocalPath;
    public static string? GetDirectoryName(string filePath) =>
        System.IO.Path.GetDirectoryName(filePath);
    public static string GetFileName(string path) =>
        System.IO.Path.GetFileName(path);
    public static string GetFileNameWithoutExtension(string path) =>
        System.IO.Path.GetFileNameWithoutExtension(path);
}

public class ContextLogger<T>(Logger logger)
{
    public readonly ILogger Logger = logger.ForContext<T>();
}
