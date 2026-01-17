using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using DremuChartHelper.Models;

namespace DremuChartHelper.ViewModels;

public partial class EditProjectDialogViewModel : ViewModelBase
{
    private readonly Window _window;
    private readonly ProjectInfo _originalProject;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _bpm = string.Empty;

    [ObservableProperty]
    private string _musicFilePath = string.Empty;

    [ObservableProperty]
    private string _projectPath = string.Empty;

    public EditProjectDialogViewModel()
    {
        throw new Exception("Error");
    }

    public EditProjectDialogViewModel(Window window, ProjectInfo project)
    {
        _window = window;
        _originalProject = project;

        // 初始化字段为现有项目的值
        ProjectName = project.Name;
        Bpm = project.Bpm;
        ProjectPath = project.Path;

        // 音乐文件路径暂时显示为空（因为在ProjectInfo中没有存储）
        // 未来如果需要在ProjectInfo中存储音乐文件信息，可以添加
        MusicFilePath = string.Empty;
    }

    [RelayCommand]
    private void Cancel()
    {
        _window.Close(null);
    }

    [RelayCommand]
    private void Confirm()
    {
        // 打印信息到控制台，方便用户查看
        Console.WriteLine("========================================");
        Console.WriteLine("编辑项目信息：");
        Console.WriteLine($"  原项目名称: {_originalProject.Name}");
        Console.WriteLine($"  新项目名称: {ProjectName}");
        Console.WriteLine($"  原BPM: {_originalProject.Bpm}");
        Console.WriteLine($"  新BPM: {Bpm}");
        Console.WriteLine($"  项目路径: {ProjectPath}");
        Console.WriteLine("========================================");

        _window.Close(new
        {
            ProjectName,
            Bpm,
            ProjectPath
        });
    }
}