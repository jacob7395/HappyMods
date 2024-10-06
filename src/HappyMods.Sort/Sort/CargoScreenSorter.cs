using System.Collections.Generic;
using System.Linq;
using HappyMods.Core.Config;
using HappyMods.Sort.Config;

namespace HappyMods.Sort.Sort;

public class CargoScreenSorter(ConfigFactory configFactory)
{
    private void Sort(MagnumCargo magnumCargo, ItemStorage activeTab, SpaceTime spaceTime)
    {
        SortItemTabMappingConfig? tabMappings = configFactory.GetConfig<SortItemTabMappingConfig>();

        List<ItemStorage> shipStorages = magnumCargo.ShipCargo.ToList();

        Debug.Log($"Found {activeTab.Items.Count} on active tab");
        
        for (int cargoIndex = 0; cargoIndex < magnumCargo.ShipCargo.Count; cargoIndex++)
        {
            ItemStorage? cargo = magnumCargo.ShipCargo[cargoIndex];
            foreach (var item in cargo.Items.ToList())
            {
                if (tabMappings?.TabMaps.Match(item) is not {} matchTab)
                {
                    Debug.Log($"Item {item.Id} not found in config");
                    continue;
                }

                var targetTabIndex = matchTab - 1;

                if (targetTabIndex == cargoIndex) continue;

                if (targetTabIndex <= shipStorages.Count || targetTabIndex < 0)
                {
                    Debug.Log($"Item {item.Id} has got invalid target tab {targetTabIndex}");
                }

                activeTab.Remove(item);
                shipStorages[targetTabIndex].ExpandHeightAndPutItem(item);

                Debug.Log($"Item {item.Id} moved to tab {targetTabIndex + 1}");
            }
        }

        foreach (ItemStorage cargo in magnumCargo.ShipCargo)
        {
            cargo.SortWithExpandByTypeAndName(spaceTime);
        }
    }

    public void Test()
    {
        
    }
    
    public void ProcessSortLoop(ScreenWithShipCargo instance, MagnumCargo magnumCargo, State state)
    {
        if (!instance.gameObject.activeSelf || 
            SharedUi.ManageSkullWindow.IsViewActive || 
            SharedUi.NarrativeTextScreen.IsViewActive)
        {
            return;
        }

        if (state.Get<SpaceTime>() is not { } spaceTime ||
            configFactory.GetConfig<SortConfig>() is not {} config) return;

        if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("[Happy.Sort] Handling full cargo sort");

            Sort(magnumCargo, instance.GetActualFloorItems(), spaceTime);
            instance.RefreshView();
        }

        if (Input.GetKeyUp(config.TabSortKey))
        {
            Debug.Log("[Happy.Sort] Handling cargo current tab");
            instance.GetActualFloorItems().SortWithExpandByTypeAndName(spaceTime);
            instance.RefreshView();
        }
    }
}