using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker;
using DremuChartHelper.ViewModels;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;

public class ElementFilter
{
    protected ElementInformation[] Elements { get; set; }

    private static readonly object _eventLock = new();
    private static bool _isEventRegistered = false;

    /// <summary>
    /// 存储所有 ElementFilter 及其子类的实例
    /// </summary>
    private static readonly List<ElementFilter> _filters = new();

    public ElementFilter()
    {
        RegisterSyncEvent();
        RegisterFilter();
    }

    private void RegisterFilter()
    {
        lock (_eventLock)
        {
            _filters.Add(this);
            Console.WriteLine($"注册过滤器: {this.GetType().Name} (当前共 {_filters.Count} 个过滤器)");
        }
    }

    private void RegisterSyncEvent()
    {
        lock (_eventLock)
        {
            if (_isEventRegistered)
            {
                return;
            }

            MainWindowViewModel.SyncChartsAction += async () =>
            {
                try
                {
                    Console.WriteLine("开始同步谱面数据...");
                    await LoadAllFiltersAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"无法获取铺面信息:\n{e.Message}");
                    // TODO: 更好的错误处理机制
                }
            };

            _isEventRegistered = true;
        }
    }

    private async Task LoadAllFiltersAsync()
    {
        Console.WriteLine($"正在为 {_filters.Count} 个过滤器加载数据...");

        foreach (var filter in _filters)
        {
            await filter.LoadPeriodInformationsAsync();
        }
    }

    public async Task LoadPeriodInformationsAsync()
    {
        ChartInformation chartInformation = ChartInformation.Instance.Value;
        if (chartInformation.Staves == null)
        {
            Console.WriteLine("错误: Staves 为 null");
            return;
        }

        var staves = chartInformation.Staves;
        var chartInformations = from information in staves
            where information.Form == "Dremu"
            select new { information.ClassName, information.Periods };
        var informations = chartInformations.ToList();

        Console.WriteLine($"[{GetType().Name}] 找到 {informations.Count} 个 Dremu 谱面");

        foreach (var information in informations)
        {
            foreach (var period in information.Periods)
            {
                Elements = await RemoteFunction.Instance.Value.GetPeriodElements(information.ClassName, period.MethodName);
            }
        }

        Console.Out.WriteLine($"[{GetType().Name}] 加载了 {Elements.Length} 个元素");

        // 在元素加载完成后调用虚方法，让子类可以处理
        OnElementsLoaded();
    }

    /// <summary>
    /// 在元素加载完成后调用，子类可以重写此方法来处理自己的逻辑
    /// </summary>
    protected virtual void OnElementsLoaded()
    {
        // 子类可以重写此方法
    }
}