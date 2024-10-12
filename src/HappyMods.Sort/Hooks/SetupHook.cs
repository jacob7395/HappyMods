using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Config;
using HappyMods.Sort.Sort;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Debug=UnityEngine.Debug;

namespace HappyMods.Sort.Hooks;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class SetupHook
{
    public static Setup SetupInstance { get; private set; } = null!;
    public static string ModName => SetupInstance.ModName;
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    
    [Hook(ModHookType.AfterConfigsLoaded)]
    public static void AfterConfigLoaded(IModContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        Debug.Log("[Happy.Sort] Initialising");

        try
        {
            StateCache.State = context.State;
            SetupInstance = Setup.Build("Happy.Sort");
        }
        catch (Exception e)
        {
            Debug.Log("[Happy.Sort] Error thrown during setup");
            Debug.LogError(e);
        }
        finally
        {
            stopwatch.Stop();
            Debug.Log($"[Happy.Sort] Finish initialisation in {stopwatch.Elapsed}");
        }
    }
}