using System;
using System.Collections.Generic;
using HappyMods.Core.Config;
using UnityEngine.PlayerLoop;

namespace HappyMods.Sort.Config;

public class SortConfigDefaultFactory : IConfigDefaultFactory
{
    public T? CreateDefault<T>() where T : class, IConfig
    {
        // Now type switch expression in net standard :(
        if (typeof(T) == typeof(SortConfig))
        {
            return new SortConfig() as T;
        }

        if (typeof(T) == typeof(SortItemTabMappingConfig))
        {
            return SortItemTabMappingConfig.Default() as T;
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