using System;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Sort;
using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Sort.Hooks;

public class CargoRecycleTabHotKeyHook
{
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    public static CargoRecycleTabHotKey? CargoRecycleTabHotKey;
    private static ILogger? Logger { get; set; }
    
    public static void CargoRecycleTabHotKeyHookAction(IModContext context)
    {
        Logger ??= Provider.GetRequiredService<ILogger>()
                           .ForContext("SourceContext", nameof(CargoRecycleTabHotKeyHook));
        
        if (context.State.Get<SpaceUI>()?.ArsenalScreen is { IsActive: true } arsenalScreen &&
            context.State.Get<MagnumCargo>() is {} magnumCargo &&
            context.State.Get<MagnumSpaceship>() is {} magnumSpaceship)
        {
            try
            {
                CargoRecycleTabHotKey ??= Provider.GetRequiredService<CargoRecycleTabHotKey>();
                CargoRecycleTabHotKey.HandleUnderPointerHotKeys(arsenalScreen, magnumCargo, magnumSpaceship);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error thrown when trying to process sort loop");
            }
        }
    }
}