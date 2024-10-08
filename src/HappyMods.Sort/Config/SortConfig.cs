using System;
using System.Text.Json.Serialization;
using HappyMods.Core.Config;

namespace HappyMods.Sort.Config;

public record SortConfig() : IConfig
{
    public string FileName => "SortConfig";

    [JsonConstructor]
    public SortConfig(bool debugLogMatches, KeyCode sortToTabsKey, KeyCode tabSortKey) : this()
    {
        DebugLogMatches = debugLogMatches;
        SortToTabsKey = sortToTabsKey;
        TabSortKey = tabSortKey;
    }
    public bool DebugLogMatches { get; }

    public KeyCode SortToTabsKey { get; } = KeyCode.F5;

    public KeyCode TabSortKey { get; } = KeyCode.S;
}