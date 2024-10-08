using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using HappyMods.Core.Unity;
using HappyMods.Sort.Config;
using HappyMods.Sort.Sort;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Debug=UnityEngine.Debug;

namespace HappyMods.Sort.Hooks;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class SetupHook
{
    public const string ModName = "Happy.Sort";
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    
    public static void ConfigureServiceProvider()
    {
        Debug.Log("[Happy.Sort] Creating service collection");
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IModConstants, ModConstant>(_ => new(ModName));
        serviceCollection.AddSingleton<IConfigDefaultFactory, SortConfigDefaultFactory>();
        serviceCollection.AddSingleton<IConfigDefaultFactory, SortConfigDefaultFactory>();
        serviceCollection.AddSingleton<MgscDataTools>();
        serviceCollection.AddSingleton<CargoScreenSorter>();
        serviceCollection.AddSingleton<ILogger>(s => SerilogConfiguration.CreateLogger(s.GetRequiredService<IModConstants>(), "Sort"));

        ModServiceProvider.Provider = serviceCollection.BuildServiceProvider();
        
        Debug.Log("[Happy.Sort] Service collection created");
    }
    
    [Hook(ModHookType.AfterConfigsLoaded)]
    public static void AfterConfigLoaded(IModContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        Debug.Log("[Happy.Sort] Initialising");

        try
        {
            ConfigureServiceProvider();
            
            var logger = Provider.GetRequiredService<ILogger>();
            Provider.GetRequiredService<IModConstants>();
            Debug.Log("[Happy.Sort] Resolved mod constants");
            Provider.GetRequiredService<MgscDataTools>().ExportItemRecords();

            Harmony harmony = new("Happy.Sort");

            SafeHarmonyRegister<AfterRaidScreen>(harmony, nameof(AfterRaidScreen.Process), logger);
            SafeHarmonyRegister<ArsenalScreen>(harmony, nameof(ArsenalScreen.Process), logger);
            SafeHarmonyRegister<FastTradeScreen>(harmony, nameof(FastTradeScreen.Process), logger);
        }
        catch (Exception e)
        {
            Debug.Log("[Happy.Sort] Error thrown during load");
            Debug.LogError(e);
        }
        finally
        {
            stopwatch.Stop();
            Debug.Log($"[Happy.Sort] Finish initialisation in {stopwatch.Elapsed}");
        }
    }
    
    private static void SafeHarmonyRegister<TMgsc>(Harmony harmony, string methodName, ILogger logger)
    {

        try
        {
            logger.Information("Configuring harmony patch for {Name} with method {MethodName}", typeof(TMgsc).Name, methodName);
            // harmony.Patch(
            //         AccessTools.Method(typeof(Tmgsc), methodName),
            //         new(typeof(CargoScreenUtil), nameof(CargoScreenUtil.Test))
            // );
        }
        catch (Exception e)
        {
            logger.Error(e, "Error thrown while trying to load {Name} and method {MethodName}", typeof(TMgsc).Name, methodName);
        }
    }
}