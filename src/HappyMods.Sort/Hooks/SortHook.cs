using System;
using System.Linq;
using HappyMods.Core.Config;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Sort;
using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Sort.Hooks;

public class SortHook
{
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    private static ILogger? Logger { get; set; }
    public static CargoScreenSorter? CargoScreenSorter;

    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void SpaceUpdateAfterGameLoop(IModContext context)
    {
        Logger ??= Provider.GetRequiredService<ILogger>()
                           .ForContext("SourceContext", nameof(SortHook));

        if (context.State.Get<SpaceUI>()?.ArsenalScreen is { IsActive: true } arsenalScreen &&
            context.State.Get<MagnumCargo>() is {} magnumCargo &&
            context.State.Get<MagnumSpaceship>() is {} magnumSpaceship)
        {
            try
            {
                CargoScreenSorter ??= Provider.GetRequiredService<CargoScreenSorter>();
                CargoScreenSorter.ProcessSortLoop(arsenalScreen, magnumCargo, context.State);
                CargoScreenSorter.HandleUnderPointerHotKeys(arsenalScreen, magnumCargo, magnumSpaceship);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error thrown when trying to process sort loop");
                throw;
            }
        }
    }
}