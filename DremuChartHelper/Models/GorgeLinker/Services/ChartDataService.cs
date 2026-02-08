using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Services;

/// <summary>
/// 图表数据服务实现
/// 负责图表数据的加载、缓存和状态管理
/// 应用了 Service Layer Pattern + Observer Pattern
/// </summary>
public partial class ChartDataService : ObservableObject, IChartDataService
{
    private readonly IChartRepository _repository;
    private Task? _initializationTask;

    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _errorMessage = string.Empty;
    private StaffInformation[]? _staves;

    /// <summary>
    /// 构造函数 - 通过依赖注入接收仓储
    /// </summary>
    public ChartDataService(IChartRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        // 自动开始初始化
        _initializationTask = InitializeAsync();
    }

    public event EventHandler<ChartDataUpdatedEventArgs>? ChartDataUpdated;

    /// <summary>
    /// 确保数据已初始化
    /// </summary>
    public async Task EnsureInitializedAsync()
    {
        if (_initializationTask == null || _initializationTask.IsCompleted)
        {
            _initializationTask = InitializeAsync();
        }

        await _initializationTask;
    }

    /// <summary>
    /// 刷新图表数据
    /// </summary>
    public async Task RefreshChartDataAsync()
    {
        await InitializeAsync();
    }

    /// <summary>
    /// 获取当前谱面数据
    /// </summary>
    public StaffInformation[]? GetStaves()
    {
        return _staves;
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            Console.WriteLine("开始初始化谱面数据");

            var scoreInformation = await _repository.GetScoreInformationAsync();

            // 检查数据是否有效
            if (scoreInformation?.Staves == null || scoreInformation.Staves.Length == 0)
            {
                ErrorMessage = "没有可用的谱面数据";
                OnChartDataUpdated(new ChartDataUpdatedEventArgs
                {
                    Staves = null,
                    IsSuccess = false,
                    ErrorMessage = ErrorMessage
                });
                return;
            }

            _staves = scoreInformation.Staves;

            OnChartDataUpdated(new ChartDataUpdatedEventArgs
            {
                Staves = _staves,
                IsSuccess = true
            });
        }
        catch (Exception ex)
        {
            // 处理异常，可以显示错误信息给用户
            ErrorMessage = $"加载失败: {ex.Message}";
            OnChartDataUpdated(new ChartDataUpdatedEventArgs
            {
                Staves = null,
                IsSuccess = false,
                ErrorMessage = ErrorMessage
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// 触发数据更新事件
    /// </summary>
    protected virtual void OnChartDataUpdated(ChartDataUpdatedEventArgs e)
    {
        ChartDataUpdated?.Invoke(this, e);
    }
}