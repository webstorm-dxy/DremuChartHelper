using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DremuChartHelper.Models.Track;

/// <summary>
/// 时间轴轨道
/// </summary>
public partial class TimelineTrack : ObservableObject
{
    /// <summary>
    /// 轨道名称
    /// </summary>
    [ObservableProperty] private string _name;

    /// <summary>
    /// 轨道高度
    /// </summary>
    [ObservableProperty] private double _height = 60;

    /// <summary>
    /// 轨道颜色
    /// </summary>
    [ObservableProperty] private string _color;

    /// <summary>
    /// 轨道是否可见
    /// </summary>
    [ObservableProperty] private bool _isVisible = true;

    /// <summary>
    /// 轨道是否锁定
    /// </summary>
    [ObservableProperty] private bool _isLocked = false;

    /// <summary>
    /// 轨道是否可编辑（未锁定）
    /// </summary>
    public bool IsEditable => !IsLocked;

    /// <summary>
    /// 轨道上的所有片段
    /// </summary>
    public ObservableCollection<TrackClip> Clips { get; set; } = new();

    public TimelineTrack(string name, string color = "#E8E8E8")
    {
        Name = name;
        Color = color;
    }
}