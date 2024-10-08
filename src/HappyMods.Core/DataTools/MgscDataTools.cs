using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using HappyMods.Core.UnitySupport;
using MGSC;
using UnityEngine;

namespace HappyMods.Core.DataTools;

public record HappyItemRecord(string Id, string Type, string? SubType = null)
{
    public string Id { get; } = Id;
    public string Type  { get; } = Type;
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
    private readonly object _dataLock = new();
    private HappyItemRecord[]? _itemRecords;
    private readonly ILogger _logger = logger.ForContext<MgscDataTools>();

    public HappyItemRecord[] GetItemRecords()
    {
        if (_itemRecords is not null)
        {
            _logger.Information("Returning cached item records");
            return _itemRecords;
        }
        
        lock (_dataLock)
        {
            if (_itemRecords is not null)
            {
                _logger.Information("Returning cached item records");
                return _itemRecords;
            }

            _logger.Information("Item records not cached creating new records");
            _itemRecords = Data.Items.Records.Select(HappyItemRecord.FromItemRecord).ToArray();
            return _itemRecords;
        }
    }
    
    public void ExportItemRecords()
    {
        _logger.Information("Exporting data records");
        
        if (File.Exists(FilePath))
        {
            _logger.Information("File already existed deleting old file");
            File.Delete(FilePath);
        }

        if (Path.GetDirectoryName(FilePath) is not {} directoryName)
        {
            _logger.Error("Unable to get directory path from file path `{FilePath}`", FilePath);
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