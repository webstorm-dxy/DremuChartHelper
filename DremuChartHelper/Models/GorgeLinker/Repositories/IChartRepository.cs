using System.Threading.Tasks;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Repositories;

/// <summary>
/// 图表数据仓储接口 - 抽象数据访问层
/// 使用仓储模式解耦数据访问逻辑
/// </summary>
public interface IChartRepository
{
    /// <summary>
    /// 获取谱面信息
    /// </summary>
    /// <returns>包含谱面元数据和 Staves 数组的 ScoreInformation</returns>
    Task<ScoreInformation> GetScoreInformationAsync();

    /// <summary>
    /// 获取指定周期的元素
    /// </summary>
    /// <param name="staffName">谱面名称</param>
    /// <param name="periodName">周期方法名</param>
    /// <returns>元素信息数组</returns>
    Task<ElementInformation[]> GetPeriodElementsAsync(string staffName, string periodName);
}