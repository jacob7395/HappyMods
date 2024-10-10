using System;
using System.Drawing.Printing;
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
    private static bool _inProgress;

    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void SpaceUpdateAfterGameLoop(IModContext context)
    {
        Logger ??= Provider.GetRequiredService<ILogger>()
                           .ForContext("SourceContext", nameof(SortHook));
        
        if(_inProgress) Logger.Error("Loop is still in progress");
        _inProgress = true;

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
                _inProgress = false;
                Logger.Error(e, "Error thrown when trying to process sort loop");
                throw;
            }
        }
        _inProgress = false;
    }
}