using System.Linq;

namespace HappyMods.Sort.Sort;

public class CargoRecycleTabHotKey(CargoScreenSorter sorter, ILogger logger)
{
    private readonly ILogger _logger = logger.ForContext<CargoRecycleTabHotKey>();
    public bool RecyclingTabAvailable(MagnumSpaceship spaceship) => spaceship.HasStoreConstructorDepartment;

    private ItemSlot? _itemPreviouslyUnderPointer;
    private ItemSlot? SearchForItemUnderPointer(ScreenWithShipCargo screenWithShipCargo) => screenWithShipCargo._cargoItemGrid._slots.FirstOrDefault(s => s.IsPointerInside);

    public void HandleUnderPointerHotKeys(ScreenWithShipCargo screenWithShipCargo,
                                          MagnumCargo magnumCargo,
                                          MagnumSpaceship magnumSpaceship)
    {
        bool actionRequired = Input.GetMouseButtonUp(2);
        
        if (_itemPreviouslyUnderPointer?.IsPointerInside == false)
        {
            _itemPreviouslyUnderPointer = null;
        }

        _itemPreviouslyUnderPointer ??= SearchForItemUnderPointer(screenWithShipCargo);

        if (_itemPreviouslyUnderPointer is null)
        {
            if(actionRequired) logger.Warning("Unable to complete action as item not found under pointer");
            return;
        }

        var cargoTab = magnumCargo.RecyclingStorage;

        switch (actionRequired)
        {
            case true when _itemPreviouslyUnderPointer.Storage == cargoTab:
                MoveItemToCargo(_itemPreviouslyUnderPointer, magnumCargo, screenWithShipCargo);
                break;
            case true when RecyclingTabAvailable(magnumSpaceship) && !magnumCargo.RecyclingInProgress: MoveItemToRecycling(_itemPreviouslyUnderPointer, magnumCargo, screenWithShipCargo);
                break;
        }
    }
    
    private void MoveItemToCargo(ItemSlot itemSlot, MagnumCargo magnumCargo,
                                 ScreenWithShipCargo screenWithShipCargo)
    {
        BasePickupItem item = itemSlot.Item;

        _logger.Information("Handling move item to recycling for {ItemId}", item.Id);

        if (!sorter.SortItem(item, magnumCargo))
        {
            _logger.Warning("Failed to move item back into cargo");
            return;
        }

        itemSlot.IsPointerInside = false;

        if (_itemPreviouslyUnderPointer == itemSlot) _itemPreviouslyUnderPointer = null;

        screenWithShipCargo.RefreshView();
    }

    private void MoveItemToRecycling(ItemSlot itemSlot, MagnumCargo magnumCargo,
                                     ScreenWithShipCargo screenWithShipCargo)
    {
        BasePickupItem item = itemSlot.Item;

        _logger.Information("Handling move item to recycling for {ItemId}", item.Id);

        if (!magnumCargo.RecyclingStorage.TryPutItem(item, CellPosition.Zero))
        {
            _logger.Warning("Failed to add item to recycling tab");
            return;
        }

        itemSlot.IsPointerInside = false;

        if (_itemPreviouslyUnderPointer == itemSlot) _itemPreviouslyUnderPointer = null;

        screenWithShipCargo.RefreshView();
    }
}