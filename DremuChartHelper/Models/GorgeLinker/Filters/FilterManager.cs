using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using DremuChartHelper.Models.GorgeLinker.Services;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Filters;

/// <summary>
/// 过滤器管理器 - 责任链模式 + 观察者模式
/// 负责协调所有过滤器的执行
/// </summary>
public class FilterManager
{
    private readonly IChartRepository _repository;
    private readonly IChartDataService _chartDataService;
    private readonly List<IElementFilter> _filters;

    public FilterManager(
        IChartRepository repository,
        IChartDataService chartDataService,
        IEnumerable<IElementFilter> filters)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _chartDataService = chartDataService ?? throw new ArgumentNullException(nameof(chartDataService));
        _filters = new List<IElementFilter>();
        if (filters != null)
        {
            _filters.AddRange(filters);
        }

        // 订阅数据更新事件
        _chartDataService.ChartDataUpdated += OnChartDataUpdated;
    }

    /// <summary>
    /// 注册过滤器
    /// </summary>
    public void RegisterFilter(IElementFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }

        _filters.Add(filter);
        Console.WriteLine($"注册过滤器: {filter.Name} (当前共 {_filters.Count} 个过滤器)");
    }

    /// <summary>
    /// 注销过滤器
    /// </summary>
    public void UnregisterFilter(IElementFilter filter)
    {
        _filters.Remove(filter);
        Console.WriteLine($"注销过滤器: {filter.Name}");
    }

    /// <summary>
    /// 执行所有过滤器
    /// </summary>
    public async Task ExecuteFiltersAsync()
    {
        Console.WriteLine($"开始执行 {_filters.Count} 个过滤器...");

        var staves = _chartDataService.GetStaves();
        if (staves == null)
        {
            Console.WriteLine("错误: Staves 为 null");
            return;
        }

        // 获取所有 Dremu 谱面
        var dremuStaves = staves
            .Where(s => s.Form == "Dremu")
            .ToList();

        Console.WriteLine($"找到 {dremuStaves.Count} 个 Dremu 谱面");

        foreach (var staff in dremuStaves)
        {
            foreach (var period in staff.Periods)
            {
                var elements = await _repository.GetPeriodElementsAsync(staff.ClassName, period.MethodName);
                await ApplyFiltersAsync(elements);
            }
        }
    }

    /// <summary>
    /// 应用所有过滤器到元素集合
    /// </summary>
    private async Task ApplyFiltersAsync(ElementInformation[] elements)
    {
        foreach (var filter in _filters)
        {
            try
            {
                // 策略模式：每个过滤器根据自己的策略处理元素
                var filteredElements = elements.Where(filter.ShouldProcess).ToArray();
                await filter.ProcessElementsAsync(filteredElements);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"过滤器 {filter.Name} 处理失败: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 数据更新时的回调
    /// </summary>
    private async void OnChartDataUpdated(object? sender, ChartDataUpdatedEventArgs e)
    {
        if (e.IsSuccess)
        {
            await ExecuteFiltersAsync();
        }
    }
}
