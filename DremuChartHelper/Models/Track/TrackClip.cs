using CommunityToolkit.Mvvm.ComponentModel;

namespace DremuChartHelper.Models.Track;

/// <summary>
/// 轨道片段（类似于视频编辑软件中的clip）
/// </summary>
public partial class TrackClip : ObservableObject
{
    private double _zoomLevel = 50.0; // 默认缩放级别：像素/秒
    private int _bpm = 120; // 默认BPM
    private int _beatsPerBar = 4; // 默认每小节4拍
    private int _beatUnit = 4; // 默认4分音符

    /// <summary>
    /// 片段开始时间（单位：秒或帧）
    /// </summary>
    [ObservableProperty] private double _startTime;

    /// <summary>
    /// 片段持续时间
    /// </summary>
    [ObservableProperty] private double _duration;

    /// <summary>
    /// 片段名称
    /// </summary>
    [ObservableProperty] private string _name;

    /// <summary>
    /// 片段颜色
    /// </summary>
    [ObservableProperty] private string _color;

    /// <summary>
    /// 是否被选中
    /// </summary>
    [ObservableProperty] private bool _isSelected;

    /// <summary>
    /// 缩放级别（像素/秒）
    /// </summary>
    public double ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (_zoomLevel != value)
            {
                _zoomLevel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PixelLeft));
                OnPropertyChanged(nameof(PixelWidth));
            }
        }
    }

    /// <summary>
    /// BPM
    /// </summary>
    public int Bpm
    {
        get => _bpm;
        set
        {
            if (_bpm != value)
            {
                _bpm = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartTimeMusicTime));
                OnPropertyChanged(nameof(EndTimeMusicTime));
            }
        }
    }

    /// <summary>
    /// 每小节拍数
    /// </summary>
    public int BeatsPerBar
    {
        get => _beatsPerBar;
        set
        {
            if (_beatsPerBar != value)
            {
                _beatsPerBar = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartTimeMusicTime));
                OnPropertyChanged(nameof(EndTimeMusicTime));
            }
        }
    }

    /// <summary>
    /// 拍号分母（音符类型）
    /// </summary>
    public int BeatUnit
    {
        get => _beatUnit;
        set
        {
            if (_beatUnit != value)
            {
                _beatUnit = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartTimeMusicTime));
                OnPropertyChanged(nameof(EndTimeMusicTime));
            }
        }
    }

    /// <summary>
    /// 片段像素宽度
    /// </summary>
    public double PixelWidth => Duration * ZoomLevel;

    /// <summary>
    /// 片段像素位置
    /// </summary>
    public double PixelLeft => StartTime * ZoomLevel;

    /// <summary>
    /// 片段像素高度（默认轨道高度减去边距）
    /// </summary>
    public double PixelHeight => 52; // 60 - 8 (上下margin)

    /// <summary>
    /// 开始时间的音乐时间表示
    /// </summary>
    public string StartTimeMusicTime => MusicTimeConverter.SecondsToMusicTime(
        StartTime, Bpm, BeatsPerBar, BeatUnit
    ).ToString();

    /// <summary>
    /// 结束时间的音乐时间表示
    /// </summary>
    public string EndTimeMusicTime => MusicTimeConverter.SecondsToMusicTime(
        EndTime, Bpm, BeatsPerBar, BeatUnit
    ).ToString();

    /// <summary>
    /// 片段结束时间
    /// </summary>
    public double EndTime => StartTime + Duration;

    public TrackClip(double startTime, double duration, string name, string color = "#4A90E2", double zoomLevel = 50.0,
        int bpm = 120, int beatsPerBar = 4, int beatUnit = 4)
    {
        StartTime = startTime;
        Duration = duration;
        Name = name;
        Color = color;
        ZoomLevel = zoomLevel;
        Bpm = bpm;
        BeatsPerBar = beatsPerBar;
        BeatUnit = beatUnit;
    }
}