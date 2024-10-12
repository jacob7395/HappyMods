using System;
using System.Drawing.Printing;
using System.Linq;
using HappyMods.Core.Config;
using HappyMods.Core.UnitySupport;
using HappyMods.Sort.Sort;
using Microsoft.Extensions.DependencyInjection;

namespace HappyMods.Sort.Hooks;

public class CargoSortHook
{
    public static ServiceProvider Provider => ModServiceProvider.Provider;
    private static ILogger? Logger { get; set; }
    public static CargoScreenSorter? CargoScreenSorter;

    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void SpaceUpdateAfterGameLoop(IModContext context)
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
                throw;
            }
        }
    }
}