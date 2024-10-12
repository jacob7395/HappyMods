using System;
using System.Drawing.Printing;
using System.Linq;
using HappyMods.Core.Config;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Sort;
using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Sort.Hooks;

public static class SpaceUpdateAfterGameLoopHook
{

    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void Execute(IModContext context)
    {
        CargoSortHook.CargoSortHookAction(context);
        CargoRecycleTabHotKeyHook.CargoRecycleTabHotKeyHookAction(context);
    }
}

public class CargoSortHook
{
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    private static ILogger? Logger { get; set; }
    public static CargoScreenSorter? CargoScreenSorter;
    
    public static void CargoSortHookAction(IModContext context)
    {
        Logger ??= Provider.GetRequiredService<ILogger>()
                           .ForContext("SourceContext", nameof(CargoSortHook));

        if (context.State.Get<SpaceUI>()?.ArsenalScreen is { IsActive: true } arsenalScreen &&
            context.State.Get<MagnumCargo>() is {} magnumCargo)
        {
            try
            {
                CargoScreenSorter ??= Provider.GetRequiredService<CargoScreenSorter>();
                CargoScreenSorter.ProcessSortLoop(arsenalScreen, magnumCargo, context.State);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error thrown when trying to process sort loop");
            }
        }
    }
}