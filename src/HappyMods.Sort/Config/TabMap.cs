using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HappyMods.Core.DataTools;

namespace HappyMods.Sort.Config;

public record TabMap
{
    public TabMap(int tabNumber, int altTabNumber, ItemTypeMatch itemMatch)
    {
        TabNumber = tabNumber;
        AltTabNumber = altTabNumber;
        ItemMatch = itemMatch;
    }
    public int TabNumber { get; }
    public int AltTabNumber { get; }
    public ItemTypeMatch ItemMatch { get; }
    public bool Matches(BasePickupItem item) => ItemMatch.Matches(item);
    public bool Matches(HappyItemRecord item) => ItemMatch.Matches(item);
}

public record ItemTypeMatch(string Id, string Type, string SubType)
{
    public string Id { get; } = Id;
    public string Type { get; } = Type;
    public string SubType { get; } = SubType;

    public static ItemTypeMatch FromItem(HappyItemRecord item) => new(item.Id, item.Type, item.SubType ?? string.Empty);
    public bool Matches(HappyItemRecord item) =>
        item.Id == Id || item.Type == Type || item.SubType == SubType;
    public bool Matches(BasePickupItem item)
    {
        BasePickupItemRecord record = item.Record<BasePickupItemRecord>();
        HappyItemRecord happyItemRecord = HappyItemRecord.FromItemRecord(record);

        return Matches(happyItemRecord);
    }
}

public static class TabMapExtensions
{
    private static readonly object _matchLock = new();
    private static readonly ConcurrentDictionary<string, TabMap> _matchCache = new();

    public static int? Match(this IEnumerable<TabMap> tabMaps, BasePickupItem item)
    {
        if (_matchCache.TryGetValue(item.Id, out var match)) return match.TabNumber;
        
        lock (_matchLock)
        {
            if (_matchCache.TryGetValue(item.Id, out var newMatch)) return newMatch.TabNumber;
            
            BasePickupItemRecord record = item.Record<BasePickupItemRecord>();
            HappyItemRecord happyItemRecord = HappyItemRecord.FromItemRecord(record);
            
            if (tabMaps.FirstOrDefault(t => t.ItemMatch.Matches(happyItemRecord)) is not {} tabMap) return null;

            _matchCache[item.Id] = tabMap;
            return tabMap.TabNumber;
        }
    }
    
    public static int? Match(this IEnumerable<TabMap> tabMaps, HappyItemRecord happyItemRecord)
    {
        if (_matchCache.TryGetValue(happyItemRecord.Id, out var match)) return match.TabNumber;
        
        lock (_matchLock)
        {
            if (_matchCache.TryGetValue(happyItemRecord.Id, out var newMatch)) return newMatch.TabNumber;
            
            if (tabMaps.FirstOrDefault(t => t.ItemMatch.Matches(happyItemRecord)) is not {} tabMap) return null;

            _matchCache[happyItemRecord.Id] = tabMap;
            return tabMap.TabNumber;
        }
    }
}