using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker;
using DremuChartHelper.Models.GorgeLinker.Filters;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using DremuChartHelper.Models.GorgeLinker.Services;
using DremuChartHelper.ViewModels;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;

/// <summary>
/// 元素过滤器基类 - 兼容层
/// 保留原有接口以支持现有代码，内部使用重构后的新架构
/// </summary>
public class ElementFilter
{
    protected ElementInformation[] Elements { get; set; } = Array.Empty<ElementInformation>();

    private static readonly object EventLock = new();
    private static bool _isEventRegistered = false;

    /// <summary>
    /// 存储所有 ElementFilter 及其子类的实例
    /// </summary>
    private static readonly List<ElementFilter> Filters = new();

    /// <summary>
    /// 新架构的过滤器管理器（懒加载）
    /// </summary>
    private static FilterManager? _filterManager;

    /// <summary>
    /// 新架构的仓储（懒加载）
    /// </summary>
    private static IChartRepository? _repository;

    /// <summary>
    /// 新架构的服务（懒加载）
    /// </summary>
    private static IChartDataService? _chartDataService;

    public ElementFilter()
    {
        RegisterSyncEvent();
        RegisterFilter();
    }

    private void RegisterFilter()
    {
        lock (EventLock)
        {
            Filters.Add(this);
            Console.WriteLine($"注册过滤器: {this.GetType().Name} (当前共 {Filters.Count} 个过滤器)");

            // 如果这个实例实现了新接口，也注册到新管理器中
            if (this is IElementFilter elementFilter)
            {
                EnsureNewArchitectureInitialized();
                _filterManager!.RegisterFilter(elementFilter);
            }
        }
    }

    private void RegisterSyncEvent()
    {
        lock (EventLock)
        {
            if (_isEventRegistered)
            {
                return;
            }

            MainWindowViewModel.SyncChartsAction += async void () =>
            {
                try
                {
                    Console.WriteLine("开始同步谱面数据...");
                    await LoadAllFiltersAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"无法获取谱面信息:\n{e.Message}");
                }
            };

            _isEventRegistered = true;
        }
    }

    private async Task LoadAllFiltersAsync()
    {
        Console.WriteLine($"正在为 {Filters.Count} 个过滤器加载数据...");

        foreach (var filter in Filters)
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
                // 使用新的仓储
                EnsureNewArchitectureInitialized();
                Elements = await _repository!.GetPeriodElementsAsync(information.ClassName, period.MethodName);
            }
        }

        Console.Out.WriteLine($"[{GetType().Name}] 加载了 {Elements.Length} 个元素");

        // 在元素加载完成后调用虚方法，让子类可以处理
        OnElementsLoaded();
    }

    /// <summary>
    /// 确保新架构已初始化
    /// </summary>
    private static void EnsureNewArchitectureInitialized()
    {
        if (_repository == null)
        {
            _repository = new GorgeStudioChartRepository();
        }

        if (_chartDataService == null)
        {
            _chartDataService = new ChartDataService(_repository);
        }

        if (_filterManager == null)
        {
            _filterManager = new FilterManager(_repository, _chartDataService);
        }
    }

    /// <summary>
    /// 在元素加载完成后调用，子类可以重写此方法来处理自己的逻辑
    /// </summary>
    protected virtual void OnElementsLoaded()
    {
        // 子类可以重写此方法
    }
}