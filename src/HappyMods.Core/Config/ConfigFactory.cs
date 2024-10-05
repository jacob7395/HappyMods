using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HappyMods.Sort.Config;
using MGSC;
using NodaTime;
using UnityEngine;

namespace HappyMods.Core.Config;

public interface IUnityConstants
{
    public string PersistentDataPath { get; }
}

public record UnityConstants : IUnityConstants
{
    public string PersistentDataPath => Application.persistentDataPath;
}

public class ConfigFactory(string persistenceFolderName, IConfigDefaultFactory defaultFactory, IUnityConstants unityConstants)
{
    public string PersistenceFolderName => ConfigPath;
    private string ConfigPath => Path.Combine(unityConstants.PersistentDataPath, persistenceFolderName);

    private readonly ConcurrentDictionary<string, ConfigLifeTime> _configCache = new();

    private class ConfigLifeTime(IConfig config)
    {
        public IConfig Config { get; private set; } = config;
        public Instant LastUpdated { get; private set; } = SystemClock.Instance.GetCurrentInstant();

        public void UpdateConfig(IConfig config)
        {
            Config = config;
            LastUpdated = SystemClock.Instance.GetCurrentInstant();
        }
    }

    private string GetFilePath(string fileName) => Path.Combine(ConfigPath, $"{fileName}.json");

    public T? GetConfig<T>(string fileName) where T : class, IConfig
    {
        if (_configCache.TryGetValue(fileName, out var config) &&
            config is {} match &&
            match.Config is T matchConfig)
        {
            if (match.LastUpdated <= Instant.MinValue)
            {
                // refresh config
            }

            return matchConfig;
        }

        var filePath = GetFilePath(fileName);

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

    private T? CreateAndWriteConfig<T>(string fileName, string filePath) where T : class, IConfig
    {
        if (defaultFactory.CreateDefault<T>() is not {} newConfig) return default;
        WriteConfig(newConfig, filePath);
        _configCache[fileName] = new(newConfig);

        return newConfig;
    }

    public T? LoadConfig<T>(string filePath) where T : class, IConfig
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        String json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }

    private void WriteConfig<T>(T config, String filePath) where T : class, IConfig
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