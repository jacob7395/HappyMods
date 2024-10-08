using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Core.UnitySupport;

public record ModConstant(string ModName) : IModConstants
{
    public string ModName { get; } = ModName;
    public string ModFolder => Path.Combine(PersistentDataPath, ModName);
    public string PersistentDataPath => Application.persistentDataPath;
}

public static class ModServiceProvider
{
    public static ServiceProvider Provider { get; set; } = null!;
}