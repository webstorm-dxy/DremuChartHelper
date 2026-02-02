using System;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker.Filters;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;

/// <summary>
/// 判定线过滤器 - 具体策略实现
/// 过滤和处理判定线元素（Dremu.DremuMainLane）
/// </summary>
public class JudgementLineFilter : ElementFilterBase
{
    /// <summary>
    /// 过滤后的判定线元素
    /// </summary>
    public ElementInformation[] JudgementLineElements { get; private set; } = Array.Empty<ElementInformation>();

    public override string Name => "JudgementLineFilter";

    public override bool ShouldProcess(ElementInformation element)
    {
        // 只处理判定线元素
        return element.ClassName == "Dremu.DremuMainLane";
    }

    public override Task ProcessElementsAsync(ElementInformation[] elements)
    {
        if (elements == null || elements.Length == 0)
        {
            Console.WriteLine("[$$$警告: Elements 为空或未加载");
            JudgementLineElements = Array.Empty<ElementInformation>();
            return Task.CompletedTask;
        }

        JudgementLineElements = elements;

        Console.WriteLine($"[{Name}] 找到 {JudgementLineElements.Length} 个判定线元素");

        return Task.CompletedTask;
    }
}