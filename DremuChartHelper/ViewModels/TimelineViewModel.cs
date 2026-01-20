using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DremuChartHelper.Models.Track;

namespace DremuChartHelper.ViewModels;

/// <summary>
/// 时间刻度标记
/// </summary>
public class TickMark
{
    public double Position { get; set; }
    public double Width { get; set; }
    public string Label { get; set; }
}

/// <summary>
/// 时间轴视图模型
/// </summary>
public partial class TimelineViewModel : ViewModelBase
{
    /// <summary>
    /// 时间轴标尺配置
    /// </summary>
    [ObservableProperty]
    private TimelineRuler _ruler;

    /// <summary>
    /// 所有轨道
    /// </summary>
    public ObservableCollection<TimelineTrack> Tracks { get; set; }

    /// <summary>
    /// 时间刻度标记集合
    /// </summary>
    public ObservableCollection<TickMark> Ticks { get; set; }

    /// <summary>
    /// 网格线位置集合
    /// </summary>
    public ObservableCollection<TickMark> GridLines { get; set; }

    /// <summary>
    /// 总宽度（像素）
    /// </summary>
    public double TotalWidth => Ruler.TotalDuration * Ruler.ZoomLevel;

    /// <summary>
    /// 播放头位置（像素）
    /// </summary>
    public double PlayheadPosition => Ruler.CurrentTime * Ruler.ZoomLevel;

    public TimelineViewModel()
    {
        Ruler = new TimelineRuler();
        Tracks = new ObservableCollection<TimelineTrack>();
        Ticks = new ObservableCollection<TickMark>();
        GridLines = new ObservableCollection<TickMark>();

        // 添加示例数据
        LoadSampleData();
        GenerateTicks();
    }

    private void LoadSampleData()
    {
        var zoom = Ruler.ZoomLevel;

        // 创建几个示例轨道
        var track1 = new TimelineTrack("轨道 1 - 音频", "#E8E8E8");
        track1.Clips.Add(new TrackClip(0, 2, "前奏", "#4A90E2", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));
        track1.Clips.Add(new TrackClip(4, 4, "主歌", "#50E3C2", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));
        track1.Clips.Add(new TrackClip(10, 6, "副歌", "#B8E986", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));

        var track2 = new TimelineTrack("轨道 2 - 旋律", "#E8E8E8");
        track2.Clips.Add(new TrackClip(2, 6, "主旋律", "#E94B3C", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));
        track2.Clips.Add(new TrackClip(14, 3, "间奏", "#F5A623", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));

        var track3 = new TimelineTrack("轨道 3 - 和声", "#E8E8E8");
        track3.Clips.Add(new TrackClip(6, 4, "和声1", "#9013FE", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));
        track3.Clips.Add(new TrackClip(18, 2, "和声2", "#BD10E0", zoom, Ruler.Bpm, Ruler.BeatsPerBar, Ruler.BeatUnit));

        Tracks.Add(track1);
        Tracks.Add(track2);
        Tracks.Add(track3);
    }

    /// <summary>
    /// 添加新轨道
    /// </summary>
    public void AddTrack(string name)
    {
        var track = new TimelineTrack(name);
        Tracks.Add(track);
    }

    /// <summary>
    /// 删除轨道
    /// </summary>
    public void RemoveTrack(TimelineTrack track)
    {
        Tracks.Remove(track);
    }

    /// <summary>
    /// 生成时间刻度标记
    /// </summary>
    private void GenerateTicks()
    {
        Ticks.Clear();
        GridLines.Clear();

        var tickInterval = Ruler.SecondsPerBar; // 每小节一个大刻度
        var pixelPerSecond = Ruler.ZoomLevel;

        for (double t = 0; t <= Ruler.TotalDuration; t += tickInterval)
        {
            var position = t * pixelPerSecond;
            var musicTime = MusicTimeConverter.SecondsToMusicTime(
                t,
                Ruler.Bpm,
                Ruler.BeatsPerBar,
                Ruler.BeatUnit
            );

            Ticks.Add(new TickMark
            {
                Position = position,
                Width = tickInterval * pixelPerSecond,
                Label = musicTime.ToString()
            });

            GridLines.Add(new TickMark
            {
                Position = position,
                Width = 0,
                Label = ""
            });
        }
    }
}