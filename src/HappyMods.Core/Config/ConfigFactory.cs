using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using HappyMods.Core.UnitySupport;
using Serilog;
using Serilog.Events;
using UnityEngine;

namespace HappyMods.Core.Config;

public class ConfigFactory(IConfigDefaultFactory defaultFactory, IModConstants modConstants, ILogger logger)
{
    private readonly ILogger _logger = logger.ForContext<ConfigFactory>();
    protected readonly ConcurrentDictionary<string, ConfigLifeTime> ConfigCache = new();

    protected class ConfigLifeTime(IConfig config, DateTime lastWriteTime)
    {
        public IConfig Config { get; private set; } = config;
        public DateTime LastFileWrite { get; private set; } = lastWriteTime;

        public void UpdateConfig(IConfig config, DateTime lastWriteTime)
        {
            Config = config;
            LastFileWrite = lastWriteTime;
        }
    }

    protected string GetFilePath(string fileName) => Path.Combine(modConstants.ModFolder, $"{fileName}.json");

    public T? GetConfig<T>() where T : class, IConfig
    {
        string? fileName = defaultFactory.GetNameFileName<T>();

        if (fileName is null)
        {
            _logger.Error("Failed to get file name for type {ConfigType}", typeof(T).Name);
            return null;
        }

        string filePath = GetFilePath(fileName);

        // check if the config is cached
        if (ConfigCache.TryGetValue(fileName, out var config) &&
            config is {} cache &&
            cache.Config is T matchConfig)
        {
            return RefreshCachedConfig(filePath, fileName, matchConfig, cache);
        }

        if (File.Exists(filePath))
        {
            return ReadConfigFromFile<T>(filePath, fileName);
        }
        
        _logger.Warning("Config file dose not exist at {FilePath} creating new default file", filePath);

        var defaultConfig = CreateDefaultConfig<T>(fileName, filePath);

        if (defaultConfig is null) return null;
        
        return UpdateCache(fileName, defaultConfig);
    }
    private T? ReadConfigFromFile<T>(string filePath, string fileName) where T : class, IConfig
    {

        _logger.Information("File exist at {FilePath} attempting to read values", filePath);
        var existingConfig = LoadFile<T>(fileName);

        if (existingConfig is not null)
        {
            _logger.Information("Successfully read config from file");
            return UpdateCache(fileName, existingConfig);
        }
            
        _logger.Error("Unable to read config from file, returning default configuration. Deleting the file may resolve the issue");

        if (defaultFactory.CreateDefault<T>() is {} defaultConfig) return defaultConfig;
            
        Debug.LogError($"Failed to create defaults for type {typeof(T)}");
        return default;
    }

    private T UpdateCache<T>(string fileName, T newConfig) where T : IConfig
    {
        ConfigCache[fileName] = new(newConfig, File.GetLastWriteTime(fileName));
        return newConfig;
    }
    
    private T RefreshCachedConfig<T>(string filePath, string fileName, T matchConfig, ConfigLifeTime configLifeTime) where T : class, IConfig
    {

        if (!File.Exists(filePath))
        {
            _logger.Information("Config file {FileName} not found writing out cached config", fileName);
                
            WriteConfig(fileName, filePath, matchConfig);
                
            return matchConfig;
        }
        
        if (configLifeTime.LastFileWrite < File.GetLastWriteTime(fileName))
        {
            _logger.Information("Config file {FileName} has been updated since last read, refreshing values", fileName);
            LoadFile<T>(fileName);
        }

        return matchConfig;
    }

    protected T? CreateDefaultConfig<T>(string fileName, string filePath) where T : class, IConfig
    {
        if (defaultFactory.CreateDefault<T>() is not {} newConfig)
        {
            Debug.LogError($"Failed to create defaults for type {typeof(T)}");
            return default;
        }
        
        WriteConfig(fileName, filePath, newConfig);
        return newConfig;
    }
    
    private void WriteConfig<T>(string fileName, string filePath, T newConfig) where T : class, IConfig
    {

        WriteConfig(newConfig, filePath);
        ConfigCache[fileName] = new(newConfig, File.GetLastWriteTime(fileName));
    }

    protected T? LoadFile<T>(string fileName) where T : class, IConfig
    {
        var filePath = GetFilePath(fileName);

        if (!File.Exists(filePath))
        {
            _logger.Error("File at {FilePath} did not exist when trying to load config", filePath);
            return null;
        }

        string json = File.ReadAllText(filePath);
        
        _logger.Debug("Read config file ({FileName}): {Json}", fileName, json);

        try
        {
            if (JsonSerializer.Deserialize<T>(json) is not {} updatedConfig)
            {
                _logger.Error("Failed to deserialize file at {FilePath}", filePath);
                return null;
            }

            return updatedConfig;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed thrown while deserialize file at {FilePath}", filePath);
            return null;
        }
    }

    protected bool WriteConfig<T>(T config, string filePath) where T : class, IConfig
    {
        _logger.Information("Writing new config file to {FilePath} for config type {Name}", filePath, typeof(T).Name);

        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        if (Path.GetDirectoryName(filePath) is not {} directoryName)
        {
            _logger.Error("Unable to get directory path from file path `{FilePath}`", filePath);
            return false;
        }

        if (!Directory.Exists(directoryName))
        {
            _logger.Warning("Config directory dose not exist creating new directory at {DirectoryName}", directoryName);
            Directory.CreateDirectory(directoryName);
        }

        if (File.Exists(filePath))
        {
            _logger.Warning("Config file already exist at {FilePath} deleting old file before write", filePath);
            File.Delete(filePath);
        }

        try
        {
            using var streamWriter = File.CreateText(filePath);
            streamWriter.Write(json);
            streamWriter.Close();
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error was thrown while trying tow rite config file to {FilePath}", filePath);
            return false;
        }
    }
}