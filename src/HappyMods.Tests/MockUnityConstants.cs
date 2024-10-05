using HappyMods.Core.Config;
using HappyMods.Core.Unity;

namespace HappyMods.Tests;

public record MockUnityConstants : IUnityConstants
{
    public string PersistentDataPath => "./PersistentData";
}