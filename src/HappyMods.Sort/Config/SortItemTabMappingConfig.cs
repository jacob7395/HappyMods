using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json.Serialization;
using HappyMods.Core.Config;
using HappyMods.Core.DataTools;

namespace HappyMods.Sort.Config;

public static class BasePickupItemExtensions
{
    private static object _matchLock = new();
    private static ConcurrentDictionary<string, TabMap> _matchCache = new();

    public static Int32? Match(this TabMap[] tabMaps, BasePickupItem item)
    {
        if (_matchCache.TryGetValue(item.Id, out var match)) return match.TabNumber;

        lock (_matchLock)
        {
            if (_matchCache.TryGetValue(item.Id, out var newMatch)) return newMatch.TabNumber;

            if (tabMaps.FirstOrDefault(t => t.ItemMatch.Matches(item)) is not {} tabMap) return null;

            _matchCache[item.Id] = tabMap;
            return tabMap.TabNumber;
        }
    }
}

public record ItemTypeMatch(String Id, String RecordType, String SubType)
{
    public string Id { get; } = Id;
    public string RecordType { get; } = RecordType;
    public string SubType { get; } = SubType;
    public bool Matches(BasePickupItem item)
    {
        BasePickupItemRecord record = item.Record<BasePickupItemRecord>();
        HappyItemRecord happyItemRecord = HappyItemRecord.FromItemRecord(record);
        
        return happyItemRecord.Id == Id || happyItemRecord.Name == RecordType || happyItemRecord.SubType == SubType;
    }
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