namespace HappyMods.Core.Unity;

public interface IModConstants
{
    public string PersistentDataPath { get; }
    string ModName { get; }
    string ModFolder { get; }
}