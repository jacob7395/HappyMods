using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;

namespace HappyMods.Sort.Config;

public record SortItemTabMappingConfig() : IConfig
{
    public string FileName => "SortItemTabMapConfig";

    [JsonConstructor]
    public SortItemTabMappingConfig(TabMap[] tabMaps) : this()
    {
        TabMaps = tabMaps;
    }
    public TabMap[] TabMaps { get; } = [];

    public static SortItemTabMappingConfig Default(MgscDataTools dataTools, ILogger logger)
    {
        var gameItems = dataTools.GetItemRecords()
                                 .Distinct();

        List<TabMap> tabs =
        [
            new(1, 0, new("", "WeaponRecord", "")),
            new TabMap(1, 0, new("", "TurretRecord", "")),
            new TabMap(1, 0, new("", "GrenadeRecord", "")),
            new TabMap(1, 0, new("", "MineRecord", "")),
            new TabMap(2, 0, new("", "AmmoRecord", "")),
            new TabMap(3, 0, new("", "ArmorRecord", "")),
            new TabMap(3, 0, new("", "BackpackRecord", "")),
            new TabMap(3, 0, new("", "BootsRecord", "")),
            new TabMap(3, 0, new("", "LeggingsRecord", "")),
            new TabMap(3, 0, new("", "HelmetRecord", "")),
            new TabMap(3, 0, new("", "VestRecord", "")),
            new TabMap(4, 0, new("", "MedkitRecord", "")),
            new TabMap(4, 0, new("", "FoodRecord", "")),
            new TabMap(5, 0, new("", "RepairRecord", "")),
            new TabMap(6, 0, new("", "AutomapRecord", "")),
            new TabMap(6, 0, new("", "DatadiskRecord", "")),
            new TabMap(6, 0, new("", "QuasiArtifactRecord", "")),
            new TabMap(6, 0, new("", "SkullRecord", "")),
            new TabMap(6, 0, new("", "", "Container")),
            new TabMap(6, 0, new("", "", "Resource")),
            new TabMap(6, 0, new("", "", "BartherResource")),
            new TabMap(6, 0, new("", "", "QuestItem")),
        ];

        var missingTabMaps = new List<TabMap>();

        foreach (HappyItemRecord gameItem in gameItems.Where(g => tabs.Match(g) == null))
        {
            logger.Warning("Unable to match game item {GameItem} in config, adding item to missing tab", gameItem);
            missingTabMaps.Add(new(7, 0, ItemTypeMatch.FromItem(gameItem)));
        }

        foreach (HappyItemRecord gameItem in gameItems.Where(g => tabs.Match(g) == null))
        {
            logger.Error("Still unable to match {GameItem}", gameItem);
        }

        return new(
        [
            new(1, 0, new("", "WeaponRecord", "")),
            new TabMap(1, 0, new("", "WeaponRecord", "")),
            new TabMap(1, 0, new("", "TurretRecord", "")),
            new TabMap(1, 0, new("", "GrenadeRecord", "")),
            new TabMap(1, 0, new("", "MineRecord", "")),
            new TabMap(2, 0, new("", "AmmoRecord", "")),
            new TabMap(3, 0, new("", "ArmorRecord", "")),
            new TabMap(3, 0, new("", "BackpackRecord", "")),
            new TabMap(3, 0, new("", "BootsRecord", "")),
            new TabMap(3, 0, new("", "LeggingsRecord", "")),
            new TabMap(3, 0, new("", "HelmetRecord", "")),
            new TabMap(3, 0, new("", "VestRecord", "")),
            new TabMap(4, 0, new("", "MedkitRecord", "")),
            new TabMap(4, 0, new("", "FoodRecord", "")),
            new TabMap(5, 0, new("", "RepairRecord", "")),
            new TabMap(6, 0, new("", "AutomapRecord", "")),
            new TabMap(6, 0, new("", "DatadiskRecord", "")),
            new TabMap(6, 0, new("", "QuasiArtifactRecord", "")),
            new TabMap(6, 0, new("", "SkullRecord", "")),
            new TabMap(6, 0, new("", "", "Container")),
            new TabMap(6, 0, new("", "", "Resource")),
            new TabMap(6, 0, new("", "", "BartherResource")),
            new TabMap(6, 0, new("", "", "QuestItem")),
            ..missingTabMaps
        ]);

    }
}