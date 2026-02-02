using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models.GorgeLinker;
using DremuChartHelper.Models.GorgeLinker.ChartInformationFilter;

namespace DremuChartHelper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public delegate void SyncChartsInvoke();

    public static SyncChartsInvoke? SyncChartsAction;

    // 在构造时初始化过滤器，过滤器会自动注册
    private static readonly ElementFilter _elementFilter = new();
    private static readonly JudgementLineFilter _judgementLineFilter = new();

    public MainWindowViewModel()
    {

    }

    [RelayCommand]
    public async Task SyncCharts()
    {
        var instance = ChartInformation.Instance.Value;
        await instance.EnsureInitializedAsync();

        if (instance.Staves == null)
        {
            throw new Exception("错误: Staves 未初始化");
        }

        // 触发同步事件，所有已注册的过滤器都会被调用
        SyncChartsAction?.Invoke();
    }
}