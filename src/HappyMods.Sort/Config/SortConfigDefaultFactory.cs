using HappyMods.Core.Config;

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
}