namespace HappyMods.Core.Config;

public interface IConfigDefaultFactory
{
    T? CreateDefault<T>() where T : class, IConfig;
}