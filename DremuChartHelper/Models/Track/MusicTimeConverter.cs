namespace DremuChartHelper.Models.Track;

/// <summary>
/// 音乐时间位置（小节:拍/分母格式）
/// </summary>
public class MusicTime
{
    /// <summary>
    /// 小节号（从1开始）
    /// </summary>
    public int Bar { get; set; }

    /// <summary>
    /// 拍号分子（当前小节的第几拍，从1开始）
    /// </summary>
    public int Beat { get; set; }

    /// <summary>
    /// 拍号分母（将一拍分为多少份）
    /// </summary>
    public int Division { get; set; }

    public MusicTime(int bar, int beat, int division)
    {
        Bar = bar;
        Beat = beat;
        Division = division;
    }

    /// <summary>
    /// 格式化为 "小节:拍/分母" 格式
    /// </summary>
    public override string ToString()
    {
        return $"{Bar}:{Beat}/{Division}";
    }
}

/// <summary>
/// 音乐时间转换工具类
/// </summary>
public static class MusicTimeConverter
{
    /// <summary>
    /// 将秒转换为音乐时间位置
    /// </summary>
    /// <param name="seconds">时间（秒）</param>
    /// <param name="bpm">BPM</param>
    /// <param name="beatsPerBar">每小节拍数</param>
    /// <param name="division">分母（将一拍分为多少份，默认4即四分音符）</param>
    /// <returns>音乐时间位置</returns>
    public static MusicTime SecondsToMusicTime(double seconds, int bpm, int beatsPerBar, int division = 4)
    {
        var secondsPerBeat = 60.0 / bpm;
        var totalBeats = seconds / secondsPerBeat;

        // 计算小节号（从1开始）
        var bar = (int)(totalBeats / beatsPerBar) + 1;

        // 计算当前小节内的拍数（从1开始）
        var beatsInCurrentBar = totalBeats % beatsPerBar;
        var beat = (int)beatsInCurrentBar + 1;

        // 计算拍内的细分位置
        var fractionalBeat = beatsInCurrentBar - (int)beatsInCurrentBar;
        var subdivision = (int)(fractionalBeat * division) + 1;

        return new MusicTime(bar, beat, division);
    }

    /// <summary>
    /// 将音乐时间位置转换为秒
    /// </summary>
    /// <param name="musicTime">音乐时间位置</param>
    /// <param name="bpm">BPM</param>
    /// <param name="beatsPerBar">每小节拍数</param>
    /// <returns>时间（秒）</returns>
    public static double MusicTimeToSeconds(MusicTime musicTime, int bpm, int beatsPerBar)
    {
        var secondsPerBeat = 60.0 / bpm;

        // 计算完整小节的秒数
        var totalSeconds = (musicTime.Bar - 1) * beatsPerBar * secondsPerBeat;

        // 加上当前小节的拍数
        totalSeconds += (musicTime.Beat - 1) * secondsPerBeat;

        // 加上细分位置
        var fractionalBeat = (double)(musicTime.Division - 1) / musicTime.Division;
        totalSeconds += fractionalBeat * secondsPerBeat;

        return totalSeconds;
    }
}