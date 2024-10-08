using System;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Sort;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Sort.Config;

public class Setup
{
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    public string ModName { get; }
    
    public void ConfigureServiceProvider()
    {
        Debug.Log("[Happy.Sort] Creating service collection");
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IModConstants, ModConstant>(_ => new(ModName));
        serviceCollection.AddSingleton<IConfigDefaultFactory, SortConfigDefaultFactory>();
        serviceCollection.AddSingleton<MgscDataTools>();
        serviceCollection.AddSingleton<CargoScreenSorter>();
        serviceCollection.AddSingleton<ConfigFactory>();
        serviceCollection.AddSingleton<ILogger>(s => SerilogConfiguration.CreateLogger(s.GetRequiredService<IModConstants>(), "Sort"));

        ModServiceProvider.Provider = serviceCollection.BuildServiceProvider();
        
        Debug.Log("[Happy.Sort] Service collection created");
    }

    public static Setup Build(string modName) => new Setup(modName);
    
    private Setup(string modName)
    {
        ModName = modName;

        ConfigureServiceProvider();
        
        var logger = Provider.GetRequiredService<ILogger>().ForContext<Setup>();
        
        Provider.GetRequiredService<MgscDataTools>().ExportItemRecords();

        Harmony harmony = new("Happy.Sort");

        SafeHarmonyRegister<AfterRaidScreen>(harmony, nameof(AfterRaidScreen.Process), logger);
        SafeHarmonyRegister<ArsenalScreen>(harmony, nameof(ArsenalScreen.Process), logger);
        SafeHarmonyRegister<FastTradeScreen>(harmony, nameof(FastTradeScreen.Process), logger);
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