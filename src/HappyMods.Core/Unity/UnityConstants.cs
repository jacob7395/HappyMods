using UnityEngine;

namespace HappyMods.Core.Unity;

public record UnityConstants : IUnityConstants
{
    public string PersistentDataPath => Application.persistentDataPath;
}