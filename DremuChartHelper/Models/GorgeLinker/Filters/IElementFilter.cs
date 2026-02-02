using System.Threading.Tasks;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Filters;

/// <summary>
/// 元素过滤器接口 - 策略模式
/// 定义元素过滤和处理的行为契约
/// </summary>
public interface IElementFilter
{
    /// <summary>
    /// 过滤器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 处理加载的元素
    /// </summary>
    /// <param name="elements">要处理的元素数组</param>
    Task ProcessElementsAsync(ElementInformation[] elements);

    /// <summary>
    /// 判断是否应该处理此元素类型
    /// </summary>
    /// <param name="element">要判断的元素</param>
    /// <returns>如果应该处理返回 true，否则返回 false</returns>
    bool ShouldProcess(ElementInformation element);
}