using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DremuChartHelper.Models.DataManager;

public class RecentProjectData
{
    public List<Dictionary<string,Dictionary<string,string>>> Projects { get; set; } = new();
}

public class RecentProjectDataManager
{
    private string SettingPath { get; }

    public RecentProjectDataManager()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "DremuChartHelper");
        Directory.CreateDirectory(appFolder);
        SettingPath = Path.Combine(appFolder, "editor_data.json");
    }

    public async Task SaveDataAsync(RecentProjectData datas)
    {
        var json = JsonSerializer.Serialize(datas);
        await File.WriteAllTextAsync(SettingPath, json);
    }
    
    public async Task<RecentProjectData> LoadDataAsync()
    {
        if (!File.Exists(SettingPath))
        {
            return new RecentProjectData();
        }

        var json = await File.ReadAllTextAsync(SettingPath);
        return JsonSerializer.Deserialize<RecentProjectData>(json) ?? new RecentProjectData();
    }
}