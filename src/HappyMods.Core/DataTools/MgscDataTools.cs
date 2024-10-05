using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using HappyMods.Core.Unity;
using MGSC;
using UnityEngine;

namespace HappyMods.Core.DataTools;

public record HappyItemRecord(String Id, String Name, String? SubType = null)
{
    public string Id { get; } = Id;
    public string Name  { get; } = Name;
    public string? SubType  { get; } = SubType;

    public static HappyItemRecord FromItemRecord(BasePickupItemRecord itemRecord)
    {
        if (itemRecord is CompositeItemRecord c)
        {
            itemRecord = c.PrimaryRecord;
        }

        if (itemRecord is TrashRecord trash) return new(itemRecord.Id, itemRecord.GetType().Name, trash.SubType.ToString());

        return new(itemRecord.Id, itemRecord.GetType().Name);
    }
}

public class MgscDataTools(string persistenceFolderName, IUnityConstants unityConstants)
{
    public const string ItemRecordFileName = "ItemDataExport.csv";
    public string PersistenceFolderName => ConfigPath;
    public string FilePath => Path.Combine(ConfigPath, ItemRecordFileName);
    public string ConfigPath => Path.Combine(unityConstants.PersistentDataPath, persistenceFolderName);
    private object _dataLock = new();
    private HappyItemRecord[]? _itemRecords;
    
    public HappyItemRecord[] GetItemRecords()
    {
        if (_itemRecords is not null) return _itemRecords;

        lock (_dataLock)
        {
            if (_itemRecords is not null) return _itemRecords;
            
            _itemRecords = Data.Items.Records.Select(HappyItemRecord.FromItemRecord).ToArray();
            return _itemRecords;
        }
    }
    
    public void ExportItemRecords()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        if (Path.GetDirectoryName(FilePath) is not {} directoryName)
        {
            Debug.LogError($"Unable to get directory path from file path `{FilePath}`");
            return;
        }

        Directory.CreateDirectory(directoryName);
    
        HappyItemRecord[] records = GetItemRecords();

        using (var writer = new StreamWriter(FilePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }
    }
}