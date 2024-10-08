using System;
using System.Collections.Generic;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using UnityEngine.PlayerLoop;

namespace HappyMods.Sort.Config;

public class SortConfigDefaultFactory(MgscDataTools mgscDataTools, ILogger logger) : IConfigDefaultFactory
{
    public T? CreateDefault<T>() where T : class, IConfig
    {
        // Type switch expression in net standard :(
        if (typeof(T) == typeof(SortConfig))
        {
            return new SortConfig() as T;
        }

        if (typeof(T) == typeof(SortItemTabMappingConfig))
        {
            return SortItemTabMappingConfig.Default(mgscDataTools, logger) as T;
        }

        return null;
    }

    protected object NameLock = new();
    protected readonly Dictionary<Type, string> NameCache = new();
    public string? GetNameFileName<T>() where T : class, IConfig
    {
        var type = typeof(T);

        if (NameCache.TryGetValue(type, out var name)) return name;
        
        lock (NameLock)
        {
            if (CreateDefault<T>() is not {} d) return null;
            
            if (NameCache.TryGetValue(type, out var newName)) return newName;

            newName = d.FileName;

            NameCache[type] = newName;
            
            return newName;
        }
    }
}