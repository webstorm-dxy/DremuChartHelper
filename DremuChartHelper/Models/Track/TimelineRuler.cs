using CommunityToolkit.Mvvm.ComponentModel;

namespace DremuChartHelper.Models.Track;

/// <summary>
/// 时间轴标尺配置
/// </summary>
public partial class TimelineRuler : ObservableObject
{
    /// <summary>
    /// 总时长（秒）
    /// </summary>
    [ObservableProperty] private double _totalDuration = 60.0;

    /// <summary>
    /// 缩放级别（像素/秒）
    /// </summary>
    [ObservableProperty] private double _zoomLevel = 50.0;

    /// <summary>
    /// 水平滚动偏移
    /// </summary>
    [ObservableProperty] private double _horizontalOffset = 0.0;

    /// <summary>
    /// 当前播放头位置（秒）
    /// </summary>
    [ObservableProperty] private double _currentTime = 0.0;

    /// <summary>
    /// 小刻度间隔（秒）
    /// </summary>
    public double SmallTickInterval => 0.5;

    /// <summary>
    /// 大刻度间隔（秒）
    /// </summary>
    public double LargeTickInterval => 5.0;
}