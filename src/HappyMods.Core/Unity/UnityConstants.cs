using System.ComponentModel.Design;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace HappyMods.Core.Unity;

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