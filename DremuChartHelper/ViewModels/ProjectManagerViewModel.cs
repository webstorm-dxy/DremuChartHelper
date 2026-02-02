using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models;
using DremuChartHelper.Models.Repositories;
using DremuChartHelper.Models.Services;
using DremuChartHelper.Views;

namespace DremuChartHelper.ViewModels;

/// <summary>
/// 项目管理器视图模型 - 重构后
/// 使用服务层进行业务逻辑处理，向后兼容
/// </summary>
public partial class ProjectManagerViewModel : ViewModelBase
{
    private readonly IProjectService _projectService;

    public ObservableCollection<ProjectInfo> Projects { get; } = new();

    public bool HasProjects => Projects.Count > 0;

    public ProjectManagerViewModel()
    {
        // 向后兼容：创建服务实例
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = System.IO.Path.Combine(appDataPath, "DremuChartHelper");
        var filePath = System.IO.Path.Combine(appFolder, "editor_data.json");

        var repository = new ProjectJsonRepository(filePath);
        _projectService = new ProjectService(repository);

        Projects.CollectionChanged += OnProjectsChanged;
        _ = LoadProjectsAsync();
    }

    /// <summary>
    /// 构造函数 - 支持依赖注入
    /// </summary>
    public ProjectManagerViewModel(IProjectService projectService)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        Projects.CollectionChanged += OnProjectsChanged;
        _ = LoadProjectsAsync();
    }

    private void OnProjectsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasProjects));
    }

    private async Task LoadProjectsAsync()
    {
        try
        {
            var projects = await _projectService.GetAllProjectsAsync();

            // 清空现有项目列表
            Projects.Clear();

            // 加载保存的项目
            foreach (var project in projects)
            {
                Projects.Add(project);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载项目失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task EditProjectAsync(ProjectInfo project)
    {
        // 获取主窗口作为对话框的父窗口
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow is not Window mainWindow)
            return;

        var dialog = new EditProjectDialog { Project = project };
        var result = await dialog.ShowDialog<object>(mainWindow);

        if (result != null)
        {
            // 使用 dynamic 获取匿名对象的属性
            dynamic dynamicResult = result;

            // 更新项目信息
            project.Name = dynamicResult.ProjectName;
            project.Bpm = dynamicResult.Bpm;

            try
            {
                // 通过服务层更新
                await _projectService.UpdateProjectAsync(project);

                // 刷新项目列表显示（通过触发属性更改通知）
                OnPropertyChanged(nameof(Projects));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新项目失败: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private void LoadProject(ProjectInfo project)
    {
        // TODO: 实现加载项目逻辑
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

            try
            {
                // 通过服务层创建项目（包含业务规则验证）
                var newProject = await _projectService.CreateProjectAsync(
                    dynamicResult.ProjectName,
                    dynamicResult.Bpm,
                    dynamicResult.ProjectPath
                );

                // 添加到项目集合
                Projects.Add(newProject);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建项目失败: {ex.Message}");
            }
        }
    }
}