using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models;
using DremuChartHelper.Models.DataManager;

namespace DremuChartHelper.ViewModels;

public partial class ProjectManagerViewModel : ViewModelBase
{
    private readonly RecentProjectDataManager _manager = new();

    public ObservableCollection<ProjectInfo> Projects { get; } = new();

    public ProjectManagerViewModel()
    {
        _ = LoadProjectsAsync();
    }

    private async Task LoadProjectsAsync()
    {
        var data = await _manager.LoadDataAsync();
        foreach (var project in data.Projects)
        {
            Projects.Add(project);
        }

        // ========== 测试数据开始 (测试完成后可删除) ==========
        AddTestData();
        // ========== 测试数据结束 ==========
    }

    // ========== 测试数据方法 (测试完成后可删除) ==========
    private void AddTestData()
    {
        Projects.Add(new ProjectInfo
        {
            Name = "Test Project 1",
            Path = "C:\\Users\\Test\\Project1",
            Bpm = "120"
        });

        Projects.Add(new ProjectInfo
        {
            Name = "动感光圈",
            Path = "D:\\Charts\\动感光圈",
            Bpm = "180"
        });

        Projects.Add(new ProjectInfo
        {
            Name = "Sample Beatmap",
            Path = "E:\\Rhythm\\Samples\\Beatmap_v2",
            Bpm = "145.5"
        });

        for (int i = 0; i < 100; i++)
        {
            Projects.Add(new ProjectInfo
            {
                Name = $"Sample Beatmap{i}",
                Path = "E:\\Rhythm\\Samples\\Beatmap_v2",
                Bpm = "145.5"
            });
        }
    }
    // ========== 测试数据方法结束 ==========

    [RelayCommand]
    private void EditProject(ProjectInfo project)
    {
        // TODO: Implement edit project logic
    }

    [RelayCommand]
    private void LoadProject(ProjectInfo project)
    {
        // TODO: Implement load project logic
    }
}