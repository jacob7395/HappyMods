using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using HappyMods.Core.Unity;
using UnityEngine;

namespace HappyMods.Core.Config;

public class PersistenceFolder(string persistenceFolderName, IUnityConstants unityConstants)
{
    public string PersistenceFolderName => ConfigPath;
    public string ConfigPath => Path.Combine(unityConstants.PersistentDataPath, persistenceFolderName);
}

public class ConfigFactory(string persistenceFolderName, IConfigDefaultFactory defaultFactory, IUnityConstants unityConstants)
{
    public string PersistenceFolderName => ConfigPath;
    protected string ConfigPath => Path.Combine(unityConstants.PersistentDataPath, persistenceFolderName);

    protected readonly ConcurrentDictionary<string, ConfigLifeTime> _configCache = new();

    protected class ConfigLifeTime(IConfig config)
    {
        public IConfig Config { get; private set; } = config;
        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

        public void UpdateConfig(IConfig config)
        {
            Config = config;
            LastUpdated = DateTime.UtcNow;
        }
    }

    protected string GetFilePath(string fileName) => Path.Combine(ConfigPath, $"{fileName}.json");

    public T? GetConfig<T>() where T : class, IConfig
    {
        if (_configCache.TryGetValue(fileName, out var config) &&
            config is {} match &&
            match.Config is T matchConfig)
        {
            if (match.LastUpdated <= DateTime.MinValue)
            {
                // refresh config
            }

            return matchConfig;
        }

        String filePath = GetFilePath(fileName);

        if (LoadConfig<T>(filePath) is {} existingFileConfig)
        {
            _configCache.TryAdd(fileName, new(existingFileConfig));
            return existingFileConfig;
        }

        if (defaultFactory.CreateDefault<T>() is not {} newConfig) return default;
        WriteConfig(newConfig, filePath);
        _configCache[fileName] = new(newConfig);

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
        _configCache[fileName] = new(newConfig);

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