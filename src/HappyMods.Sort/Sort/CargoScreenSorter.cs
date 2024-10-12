using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;
using HappyMods.Sort.Config;

namespace HappyMods.Sort.Sort;

public class CargoScreenSorter(ConfigFactory configFactory, ILogger logger)
{
    private readonly ILogger _logger = logger.ForContext<CargoScreenSorter>();
    private void Sort(MagnumCargo magnumCargo, ItemStorage activeTab, SpaceTime spaceTime, 
                      bool sortRecycling)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        _logger.Information("Processing full cargo sort, include recycling tab set to {SortRecycling}", sortRecycling);
        
        SortItemTabMappingConfig? tabMappings = configFactory.GetConfig<SortItemTabMappingConfig>();
        
        if (tabMappings is null)
        {
            _logger.Error("Unable to load tab mappings config file, aborting sort");
            return;
        }

        ItemStorage[] shipStorages = magnumCargo.ShipCargo.ToArray();

        int totalSortCount = 0;
        HashSet<ItemStorage> changedItemStorageTabs = new();

        foreach (var (tabIndex, itemStorage, item) in magnumCargo.ShipCargo
                                                                   .SelectMany((itemStorage, index) => 
                                                                       itemStorage.Items
                                                                                  .Select(item => (index, itemStorage, item))
                                                                                  .ToArray())
                                                                   .ToArray())
        {
            if (!SortTab(item, tabIndex, itemStorage, tabMappings, shipStorages)) { continue; }
            
            changedItemStorageTabs.Add(itemStorage);
            totalSortCount++;
        }

        int recyclingMoved = 0;
        var recyclingStorage = magnumCargo.RecyclingStorage;
        if (sortRecycling && magnumCargo.RecyclingInProgress == false)
        {
            foreach (var item in magnumCargo.RecyclingStorage.Items.ToList())
            {
                if (!SortTab(item, 8, recyclingStorage, tabMappings, shipStorages)) { continue; }
            
                recyclingMoved++;
            }
        }

        foreach (ItemStorage cargo in changedItemStorageTabs)
        {
            cargo.SortWithExpandByTypeAndName(spaceTime);
        }
        
        stopwatch.Stop();
        _logger.Information("Sorted {ItemCount} for cargo tabs in {StopwatchElapsed}", totalSortCount, stopwatch.Elapsed);
        
        if(sortRecycling) _logger.Information("Moved {ItemCount} from recycling", recyclingMoved);
    }
    
    private bool SortTab(BasePickupItem item, int tabIndex, ItemStorage activeTab,
                         SortItemTabMappingConfig? tabMappings, 
                         ItemStorage[] shipStorages)
    {
        if (tabMappings?.TabMaps.Match(item) is not {} matchTab)
        {
            BasePickupItemRecord record = item.Record<BasePickupItemRecord>();
            HappyItemRecord happyItemRecord = HappyItemRecord.FromItemRecord(record);
            
            _logger.Information("Item {Id}, {Name}, {SubType} not found in config", happyItemRecord.Id, happyItemRecord.Type, happyItemRecord.SubType);
            matchTab = 7;
        }

        int targetTabIndex = matchTab - 1;

        if (targetTabIndex == tabIndex) return false;
            
        if (matchTab > shipStorages.Length || targetTabIndex < 0)
        {
            _logger.Information("Item {ItemId} has got invalid target tab {MatchTab}, sending item to final tab", item.Id, matchTab);
            targetTabIndex = shipStorages.Length - 1;
        }

        shipStorages[targetTabIndex].ExpandHeightAndPutItem(item);

        _logger.Debug("Item {ItemId} moved to tab {TargetTabIndex}", item.Id, targetTabIndex + 1);
        return true;
    }

    public void ProcessSortLoop(ScreenWithShipCargo instance, MagnumCargo magnumCargo, State state)
    {
        if (!instance.gameObject.activeSelf || 
            SharedUi.ManageSkullWindow.IsViewActive || 
            SharedUi.NarrativeTextScreen.IsViewActive)
        {
            _logger.Information("No cargo windows active");
            return;
        }
        
        if (state.Get<SpaceTime>() is not { } spaceTime ||
            configFactory.GetConfig<SortConfig>() is not {} config) return;
        
        if (Input.GetKeyUp(KeyCode.P))
        {
            bool sortRecycling = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift);
            Sort(magnumCargo, instance.GetActualFloorItems(), spaceTime, sortRecycling);
            instance.RefreshView();
        }

        if (Input.GetKeyUp(config.TabSortKey))
        {
            _logger.Information("Processing sort");
            instance.GetActualFloorItems().SortWithExpandByTypeAndName(spaceTime);
            instance.RefreshView();
        }
    }
}