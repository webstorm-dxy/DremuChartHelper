using System;
using System.Reflection;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models.GorgeLinker.Services;

namespace DremuChartHelper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IChartDataService _chartDataService;

    public MainWindowViewModel(IChartDataService chartDataService)
    {
        _chartDataService = chartDataService ?? throw new System.ArgumentNullException(nameof(chartDataService));
    }

    [RelayCommand]
    public async Task SyncCharts()
    {
        await _chartDataService.RefreshChartDataAsync();
    }
}
