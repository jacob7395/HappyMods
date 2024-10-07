using System;
using System.Diagnostics.CodeAnalysis;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using HappyMods.Core.Unity;
using HappyMods.Sort.Config;
using HappyMods.Sort.Sort;
using HarmonyLib;

namespace HappyMods.Sort.Hooks;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class SetupHook
{
    public const string ModName = "Happy.Sort";
    public static readonly ConfigFactory ConfigFactory;
    public static readonly MgscDataTools MgscDataTools;
    public static readonly CargoScreenSorter CargoScreenSorter;

    static SetupHook()
    {
        var unityConstants = new UnityConstants();
        var sortConfigDefaultFactory = new SortConfigDefaultFactory();

        ConfigFactory = new(ModName, sortConfigDefaultFactory, unityConstants);
        MgscDataTools = new(ModName, unityConstants);
        CargoScreenSorter = new(ConfigFactory);
    }
    
    [Hook(ModHookType.AfterConfigsLoaded)]
    public static void AfterConfigLoaded(IModContext context)
    {
        Debug.Log("Happy.Sort initialising");

        try
        {
            MgscDataTools.ExportItemRecords();
            
            Harmony harmony = new("Happy.Sort");
            
            SafeHarmonyRegister<AfterRaidScreen>(harmony, nameof(AfterRaidScreen.Process));
            SafeHarmonyRegister<ArsenalScreen>(harmony, nameof(ArsenalScreen.Process));
            SafeHarmonyRegister<FastTradeScreen>(harmony, nameof(FastTradeScreen.Process));

            var state = context.State.Get<ArsenalScreen>();
            Debug.Log((state is not null).ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Happy.Sort threw error error during load");
            Debug.LogError(e);
            return;
        }
        
        Debug.Log("Happy.Sort initialised");
    }
    
    private static void SafeHarmonyRegister<TMgsc>(Harmony harmony, string methodName)
    {

        try
        {
            // harmony.Patch(
            //         AccessTools.Method(typeof(Tmgsc), methodName),
            //         new(typeof(CargoScreenUtil), nameof(CargoScreenUtil.Test))
            // );
        }
        catch (Exception e)
        {
            Debug.Log($"Error thrown while trying to load {typeof(TMgsc).Name} and method {methodName}");
            Debug.LogError(e);
        }
    }
}