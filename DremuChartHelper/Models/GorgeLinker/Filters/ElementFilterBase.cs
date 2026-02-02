using System;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Filters;

/// <summary>
/// 元素过滤器基类 - 提供默认实现
/// 继承自 ElementFilter 以保持兼容性和自动注册功能
/// 继承此类可以快速创建自定义过滤器
/// </summary>
public abstract class ElementFilterBase : ElementFilter, IElementFilter
{
    /// <summary>
    /// 过滤器名称
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// 默认处理所有元素，子类可以重写
    /// </summary>
    public virtual bool ShouldProcess(ElementInformation element)
    {
        return true;
    }

    /// <summary>
    /// 处理元素 - 子类必须实现
    /// </summary>
    public abstract Task ProcessElementsAsync(ElementInformation[] elements);

    /// <summary>
    /// 重写 OnElementsLoaded 以调用新的 ProcessElementsAsync
    /// 这样可以同时兼容新旧架构
    /// </summary>
    protected override void OnElementsLoaded()
    {
        // 如果有元素，调用新的处理方法
        if (Elements != null && Elements.Length > 0)
        {
            // 过滤元素
            var filteredElements = Array.FindAll(Elements, ShouldProcess);

            // 异步处理（不等待，为了兼容性）
            _ = ProcessElementsAsync(filteredElements);
        }
    }
}