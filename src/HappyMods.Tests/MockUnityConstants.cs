using HappyMods.Core.Config;

namespace HappyMods.Tests;

public record MockUnityConstants : IUnityConstants
{
    public string PersistentDataPath => "./PersistentData";
}