using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models;
using DremuChartHelper.Models.DataManager;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using DremuChartHelper.Views;
using System.Linq;

namespace DremuChartHelper.ViewModels;

public partial class ProjectManagerViewModel : ViewModelBase
{
    private readonly RecentProjectDataManager _manager = new();

    public ObservableCollection<ProjectInfo> Projects { get; } = new();

    public bool HasProjects => Projects.Count > 0;

    public ProjectManagerViewModel()
    {
        Projects.CollectionChanged += OnProjectsChanged;
        _ = LoadProjectsAsync();
    }

    private void OnProjectsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasProjects));
    }

    private async Task LoadProjectsAsync()
    {
        var data = await _manager.LoadDataAsync();

        // 清空现有项目列表
        Projects.Clear();

        // 加载保存的项目
        foreach (var project in data.Projects)
        {
            Projects.Add(project);
        }
    }

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

    [RelayCommand]
    private async Task CreateProjectAsync()
    {
        // 获取主窗口作为对话框的父窗口
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow is not Window mainWindow)
            return;

        var dialog = new CreateProjectDialog();
        dialog.DataContext = new CreateProjectDialogViewModel(dialog);

        var result = await dialog.ShowDialog<object>(mainWindow);

        if (result != null)
        {
            // 使用 dynamic 获取匿名对象的属性
            dynamic dynamicResult = result;

            // 创建新的项目信息
            var newProject = new ProjectInfo
            {
                Name = dynamicResult.ProjectName,
                Bpm = dynamicResult.Bpm,
                Path = dynamicResult.ProjectPath
            };

            // 添加到项目集合
            Projects.Add(newProject);

            // 保存到数据管理器
            var data = new RecentProjectData { Projects = Projects.ToList() };
            await _manager.SaveDataAsync(data);
        }
    }
}