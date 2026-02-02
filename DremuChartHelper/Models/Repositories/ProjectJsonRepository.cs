using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DremuChartHelper.Models;

namespace DremuChartHelper.Models.Repositories;

/// <summary>
/// 项目 JSON 文件仓储实现
/// 将项目数据持久化到 JSON 文件
/// </summary>
public class ProjectJsonRepository : IProjectRepository
{
    private readonly string _filePath;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="filePath">JSON 文件路径</param>
    public ProjectJsonRepository(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        // 确保目录存在
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task<List<ProjectInfo>> LoadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<ProjectInfo>();
        }

        var json = await File.ReadAllTextAsync(_filePath);
        var data = JsonSerializer.Deserialize<RecentProjectData>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return data?.Projects ?? new List<ProjectInfo>();
    }

    public async Task SaveAllAsync(List<ProjectInfo> projects)
    {
        var data = new RecentProjectData { Projects = projects };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(_filePath, json);
    }

    /// <summary>
    /// 内部数据模型
    /// </summary>
    private class RecentProjectData
    {
        [JsonPropertyName("Projects")]
        public List<ProjectInfo> Projects { get; set; } = new();
    }
}