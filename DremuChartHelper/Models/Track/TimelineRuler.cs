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

    /// <summary>
    /// BPM（每分钟拍数）
    /// </summary>
    [ObservableProperty] private int _bpm = 120;

    /// <summary>
    /// 拍号分子（每小节拍数）
    /// </summary>
    [ObservableProperty] private int _beatsPerBar = 4;

    /// <summary>
    /// 拍号分母（音符类型，4=四分音符）
    /// </summary>
    [ObservableProperty] private int _beatUnit = 4;

    /// <summary>
    /// 每拍时长（秒）
    /// </summary>
    public double SecondsPerBeat => 60.0 / Bpm;

    /// <summary>
    /// 每小节时长（秒）
    /// </summary>
    public double SecondsPerBar => SecondsPerBeat * BeatsPerBar;
}