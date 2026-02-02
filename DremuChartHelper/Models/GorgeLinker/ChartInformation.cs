using System;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using DremuChartHelper.Models.GorgeLinker.Services;
using DremuChartHelper.ViewModels;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker;

/// <summary>
/// 图表信息单例 - 兼容层
/// 保留单例模式以支持现有代码，内部使用重构后的服务层
/// </summary>
public class ChartInformation
{
    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsLoading { get; set; } = true;
    public StaffInformation[]? Staves;

    public static readonly Lazy<ChartInformation> Instance = new(() => new ChartInformation());

    private readonly IChartRepository _repository;
    private readonly IChartDataService _service;
    private Task? _initializationTask;

    private ChartInformation()
    {
        // 创建仓储和服务实例（依赖注入的简化版本）
        _repository = new GorgeStudioChartRepository();
        _service = new ChartDataService(_repository);

        _initializationTask = InitializeAsync();

        // 订阅静态事件（向后兼容）
        MainWindowViewModel.SyncChartsAction += UpdateChartInformation;
    }

    /// <summary>
    /// 更新图表信息 - 向后兼容方法
    /// </summary>
    public void UpdateChartInformation()
    {
        _initializationTask = InitializeAsync();
        GC.Collect();
    }

    /// <summary>
    /// 确保已初始化 - 向后兼容方法
    /// </summary>
    public Task EnsureInitializedAsync()
    {
        return _initializationTask ?? Task.CompletedTask;
    }

    /// <summary>
    /// 初始化数据 - 内部使用新的服务层
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            // 使用新的仓储加载数据
            var scoreInformation = await _repository.GetScoreInformationAsync();

            // 检查数据是否有效
            if (scoreInformation?.Staves == null || scoreInformation.Staves.Length == 0)
            {
                ErrorMessage = "没有可用的谱面数据";
                Console.WriteLine("警告: Staves 为空或 null");
                return;
            }

            // 在 UI 线程上处理数据
            Staves = scoreInformation.Staves;
        }
        catch (Exception ex)
        {
            // 处理异常，可以显示错误信息给用户
            ErrorMessage = $"加载失败: {ex.Message}";
            Console.WriteLine($"加载失败: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}