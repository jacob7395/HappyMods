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

public class MgscDataTools(IModConstants modConstants, ILogger logger)
{
    public const string ItemRecordFileName = "ItemDataExport.csv";
    public string FilePath => Path.Combine(modConstants.ModFolder, ItemRecordFileName);
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
        logger.Information("Exporting data records");
        
        if (File.Exists(FilePath))
        {
            logger.Information("File already existed deletings old file");
            File.Delete(FilePath);
        }

        if (Path.GetDirectoryName(FilePath) is not {} directoryName)
        {
            logger.Error("Unable to get directory path from file path `{FilePath}`", FilePath);
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