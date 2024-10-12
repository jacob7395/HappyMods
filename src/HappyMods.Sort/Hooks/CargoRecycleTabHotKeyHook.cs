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
    private static bool _inProgress;
    private static State? _previousState;

    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void SpaceUpdateAfterGameLoop(IModContext context)
    {
        Logger ??= Provider.GetRequiredService<ILogger>()
                           .ForContext("SourceContext", nameof(CargoSortHook));

        if (_inProgress) Logger.Error("Loop is still in progress"); 
            
        _inProgress = true;

        if(_previousState is not null && _previousState != context.State) Logger.Warning($"State is different from last run");
        _previousState = context.State;
        
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
                _inProgress = false;
                Logger.Error(e, "Error thrown when trying to process sort loop");
                throw;
            }
        }
        _inProgress = false;
    }
}