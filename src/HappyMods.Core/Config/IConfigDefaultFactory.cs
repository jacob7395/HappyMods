namespace HappyMods.Core.Config;

public interface IConfigDefaultFactory
{
    T? CreateDefault<T>() where T : class, IConfig;
    string? GetNameFileName<T>() where T : class, IConfig;
}