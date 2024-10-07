using System;

namespace HappyMods.Sort.Hooks;

public static class SortHook
{
    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void SpaceUpdateAfterGameLoop(IModContext context)
    {
        if (context.State.Get<SpaceUI>()?.ArsenalScreen is { IsActive: true } arsenalScreen &&
            context.State.Get<MagnumCargo>() is {} magnumCargo)
        {
            try
            {
                SetupHook.CargoScreenSorter.ProcessSortLoop(arsenalScreen, magnumCargo, context.State);
            }
            catch (Exception e)
            {
                Debug.Log("[Happy.Sort] Error thrown when trying to process sort loop");
                Debug.LogError(e);
                throw;
            }
        }
    }
}