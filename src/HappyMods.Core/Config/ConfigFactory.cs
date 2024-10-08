using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using HappyMods.Core.Unity;
using UnityEngine;

namespace HappyMods.Core.Config;

public class ConfigFactory(IConfigDefaultFactory defaultFactory, IModConstants modConstants)
{

    protected readonly ConcurrentDictionary<string, ConfigLifeTime> ConfigCache = new();

    protected class ConfigLifeTime(IConfig config, DateTime lastWriteTime)
    {
        public IConfig Config { get; private set; } = config;
        public DateTime LastUpdated { get; private set; } = lastWriteTime;

        public void UpdateConfig(IConfig config, DateTime lastWriteTime)
        {
            Config = config;
            LastUpdated = lastWriteTime;
        }
    }
    protected string GetFilePath(string fileName) => Path.Combine(modConstants.ModFolder, $"{fileName}.json");

    public T? GetConfig<T>() where T : class, IConfig
    {
        string? fileName = defaultFactory.GetNameFileName<T>();

        if (fileName is null)
        {
            Debug.LogError($"Failed to get file name for type {typeof(T).Name}");
            return null;
        }
        
        if (ConfigCache.TryGetValue(fileName, out var config) &&
            config is {} match &&
            match.Config is T matchConfig)
        {
            if (match.LastUpdated <= DateTime.MinValue)
            {
                // refresh config
            }

            return matchConfig;
        }

        string filePath = GetFilePath(fileName);

        if (LoadConfig<T>(filePath) is {} existingFileConfig)
        {
            ConfigCache.TryAdd(fileName, new(existingFileConfig, File.GetLastWriteTime(fileName)));
            return existingFileConfig;
        }

        if (defaultFactory.CreateDefault<T>() is not {} newConfig) return default;
        WriteConfig(newConfig, filePath);
        ConfigCache[fileName] = new(newConfig, File.GetLastWriteTime(fileName));

        return CreateAndWriteConfig<T>(fileName, filePath);
    }

    protected T? CreateAndWriteConfig<T>(string fileName, string filePath) where T : class, IConfig
    {
        if (defaultFactory.CreateDefault<T>() is not {} newConfig)
        {
            Debug.LogError($"Failed to create defaults for type {typeof(T)}");
            return default;
        }
        
        WriteConfig(newConfig, filePath);
        ConfigCache[fileName] = new(newConfig, File.GetLastWriteTime(fileName));

        return newConfig;
    }

    protected T? LoadConfig<T>(string filePath) where T : class, IConfig
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        String json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    protected void WriteConfig<T>(T config, String filePath) where T : class, IConfig
    {
        String json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
                WriteIndented = true
        });

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        if (Path.GetDirectoryName(filePath) is not {} directoryName)
        {
            Debug.LogError($"Unable to get directory path from file path `{filePath}`");
            return;
        }

        Directory.CreateDirectory(directoryName);
        using var streamWriter = File.CreateText(filePath);
        streamWriter.Write(json);
        streamWriter.Close();
    }
}