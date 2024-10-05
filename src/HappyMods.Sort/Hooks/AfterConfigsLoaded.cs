using System;
using System.Diagnostics.CodeAnalysis;
using HappyMods.Core.Config;
using HappyMods.Sort.Config;
using HarmonyLib;

namespace HappyMods.Sort.Hooks;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class AfterConfigsLoaded
{
    public const string ModName = "Happy.Sort";
    public static readonly ConfigFactory ConfigFactory = new(ModName, 
            new SortConfigDefaultFactory(), 
            new UnityConstants());
    
    [Hook(ModHookType.AfterConfigsLoaded)]
    public static void Execute(IModContext context)
    {
        Debug.Log("Happy.Sort initialising");

        try
        {
            var config = ConfigFactory.LoadConfig<SortConfig>("SortConfig");
            
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