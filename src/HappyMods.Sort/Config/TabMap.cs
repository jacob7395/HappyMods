using System;

namespace HappyMods.Sort.Config;

public record TabMap
{
    public TabMap(Int32 tabNumber, Int32 altTabNumber, ItemTypeMatch itemMatch)
    {
        TabNumber = tabNumber;
        AltTabNumber = altTabNumber;
        ItemMatch = itemMatch;
    }
    public int TabNumber { get; }
    public int AltTabNumber { get; }
    public ItemTypeMatch ItemMatch { get; }
}