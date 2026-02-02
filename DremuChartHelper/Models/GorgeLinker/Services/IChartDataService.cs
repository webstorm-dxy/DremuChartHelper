using System;
using System.Threading.Tasks;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Services;

/// <summary>
/// 图表数据服务接口
/// 定义图表数据的加载、刷新和查询操作
/// </summary>
public interface IChartDataService
{
    /// <summary>
    /// 图表数据更新事件
    /// 当图表数据成功加载或更新时触发
    /// </summary>
    event EventHandler<ChartDataUpdatedEventArgs>? ChartDataUpdated;

    /// <summary>
    /// 确保数据已初始化
    /// 如果数据尚未加载，则加载；如果已加载，则直接返回
    /// </summary>
    Task EnsureInitializedAsync();

    /// <summary>
    /// 刷新图表数据
    /// 强制重新从服务器加载最新数据
    /// </summary>
    Task RefreshChartDataAsync();

    /// <summary>
    /// 获取当前谱面数据
    /// </summary>
    /// <returns>谱面数组，如果未加载则返回 null</returns>
    StaffInformation[]? GetStaves();

    /// <summary>
    /// 是否正在加载数据
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// 错误信息
    /// </summary>
    string ErrorMessage { get; }
}

/// <summary>
/// 图表数据更新事件参数
/// </summary>
public class ChartDataUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// 更新后的谱面数据
    /// </summary>
    public StaffInformation[]? Staves { get; set; }

    /// <summary>
    /// 是否成功更新
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 错误信息（如果失败）
    /// </summary>
    public string? ErrorMessage { get; set; }
}