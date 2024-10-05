namespace HappyMods.Sort.Hooks;

public class SpaceUpdatedGameLoop
{
    [Hook(ModHookType.SpaceUpdateAfterGameLoop)]
    public static void Execute(IModContext context)
    {
        Debug.Log("Happy.Sort initialising");
        
        if (context.State.Get<SpaceUI>()?.ArsenalScreen is { IsActive: true } arsenalScreen &&
            context.State.Get<MagnumCargo>() is {} magnumCargo)
        {
            // try
            // {
            //     CargoScreenUtil.ProcessSortLoop(arsenalScreen, magnumCargo, context.State);
            // }
            // catch (Exception e)
            // {
            //     Debug.Log("[Happy.Sort] Error thrown when trying to process sort loop");
            //     Debug.LogError(e);
            //     throw;
            // }
        }
    }
}