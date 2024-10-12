using System.Linq;

namespace HappyMods.Sort.Sort;

public class CargoRecycleTabHotKey(ILogger logger)
{
    private readonly ILogger _logger = logger.ForContext<CargoRecycleTabHotKey>();
    public bool RecyclingTabAvailable(MagnumSpaceship spaceship) => spaceship.HasStoreConstructorDepartment;
    
    private ItemSlot? ItemPreviouslyUnderPointer;
    private ItemSlot? SearchForItemUnderPointer(ScreenWithShipCargo screenWithShipCargo) => screenWithShipCargo._cargoItemGrid._slots.FirstOrDefault(s => s.IsPointerInside);
    public void HandleUnderPointerHotKeys(ScreenWithShipCargo screenWithShipCargo, MagnumCargo magnumCargo, MagnumSpaceship magnumSpaceship)
    {
        
        if (ItemPreviouslyUnderPointer?.IsPointerInside == false)
        {
            _logger.Debug("Pointer no longer in item");
            ItemPreviouslyUnderPointer = null;
        }

        ItemPreviouslyUnderPointer ??= SearchForItemUnderPointer(screenWithShipCargo);
            
        if (ItemPreviouslyUnderPointer is null) return;

        bool recyclingTabAvailable = RecyclingTabAvailable(magnumSpaceship);
        
        if (Input.GetMouseButtonUp(2) && recyclingTabAvailable && !magnumCargo.RecyclingInProgress)
        {
            _logger.Information("Pointer is in an item {RecyclingTabAvailable}", recyclingTabAvailable);
            
            BasePickupItem item = ItemPreviouslyUnderPointer.Item;
            
            _logger.Information("Handling move item to recycling for {ItemId}", item.Id);
            
            if (!magnumCargo.RecyclingStorage.TryPutItem(item, CellPosition.Zero))
            {
                _logger.Warning("Failed to add item to recycling tab");
                return;
            }

            ItemPreviouslyUnderPointer.IsPointerInside = false;
            ItemPreviouslyUnderPointer = null;           
            screenWithShipCargo.RefreshView();
        }
    }
}