using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System.IO.Compression;

namespace DremuChartHelper.ViewModels;

public partial class CreateProjectDialogViewModel : ViewModelBase
{
    private readonly Window _window;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _bpm = string.Empty;

    [ObservableProperty]
    private string _musicFilePath = string.Empty;

    [ObservableProperty]
    private string _projectPath = string.Empty;

    public CreateProjectDialogViewModel()
    {
        throw new Exception("Error");
    }

    public CreateProjectDialogViewModel(Window window)
    {
        _window = window;
    }

    [RelayCommand]
    private async Task SelectMusicFileAsync()
    {
        if (_window.StorageProvider is not { } storageProvider)
            return;

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择音乐文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("音频文件")
                {
                    Patterns = new[] { "*.mp3", "*.wav", "*.ogg", "*.flac", "*.m4a" }
                },
                new FilePickerFileType("所有文件")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0)
        {
            MusicFilePath = files[0].Path.LocalPath;
        }
    }

    [RelayCommand]
    private async Task SelectProjectPathAsync()
    {
        if (_window.StorageProvider is not { } storageProvider)
            return;

        var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择项目保存位置",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            ProjectPath = folders[0].Path.LocalPath;
        }
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
        Console.WriteLine("新建项目信息：");
        Console.WriteLine($"  项目名称: {ProjectName}");
        Console.WriteLine($"  BPM: {Bpm}");
        Console.WriteLine($"  音乐文件: {MusicFilePath}");
        Console.WriteLine($"  项目路径: {ProjectPath}");
        Console.WriteLine("========================================");

        // 创建项目 zip 文件
        var zipFileName = $"{ProjectName}.zip"; // 使用自定义扩展名
        var fullZipPath = System.IO.Path.Combine(ProjectPath, zipFileName);

        // 使用 .NET 内置的 System.IO.Compression 创建 zip 文件
        using (var archive = ZipFile.Open(fullZipPath, ZipArchiveMode.Create))
        {
            // 添加音乐文件到 zip（使用文件名作为 zip 中的条目名称）
            var musicFileName = System.IO.Path.GetFileName(MusicFilePath);
            var entry = archive.CreateEntry(musicFileName);
            using (var entryStream = entry.Open())
            using (var fileStream = System.IO.File.OpenRead(MusicFilePath))
            {
                fileStream.CopyTo(entryStream);
            }

            // TODO：添加JSON文件保存项目信息
        }

        // 更新 ProjectPath 为完整的 zip 文件路径
        ProjectPath = fullZipPath;

        _window.Close(new
        {
            ProjectName,
            Bpm,
            MusicFilePath,
            ProjectPath
        });
    }
}