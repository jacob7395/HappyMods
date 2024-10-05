using System;
using System.Text.Json.Serialization;
using HappyMods.Core.Config;

namespace HappyMods.Sort.Config;

public record ItemTypeMatch(String Id, String RecordType, String SubType)
{
    public string Id { get; } = Id;
    public string RecordType { get; } = RecordType;
    public string SubType { get; } = SubType;
}

public record SortItemTabMappingConfig() : IConfig
{
    public string FileName => "SortItemTabMapConfig";
    
    [JsonConstructor]
    public SortItemTabMappingConfig(TabMap[] tabMaps) : this()
    {
        TabMaps = tabMaps;
    }

    public TabMap[] TabMaps { get; } = [];

    public static SortItemTabMappingConfig Default()
    {
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
        ]);

    }
}