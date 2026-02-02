using System;
using System.Linq;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;

public class JudgementLineFilter : ElementFilter
{
    protected ElementInformation[] JudgementLineElements { get; private set; }

    /// <summary>
    /// 在元素加载完成后调用，过滤判定线元素
    /// </summary>
    protected override void OnElementsLoaded()
    {
        if (Elements == null || Elements.Length == 0)
        {
            Console.WriteLine("警告: Elements 为空或未加载");
            JudgementLineElements = Array.Empty<ElementInformation>();
            return;
        }

        JudgementLineElements = Elements
            .Where(element => element.ClassName == "Dremu.DremuMainLane")
            .ToArray();

        Console.Out.WriteLine($"找到 {JudgementLineElements.Length} 个判定线元素");
    }
}