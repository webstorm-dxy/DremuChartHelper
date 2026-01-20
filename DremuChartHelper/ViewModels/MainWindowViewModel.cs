using System;
using CommunityToolkit.Mvvm.Input;
using DremuChartHelper.Models.GorgeLinker;

namespace DremuChartHelper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    public ChartInformation ChartInformations;

    public delegate void SyncChartsInvoke();
    
    public static SyncChartsInvoke? SyncChartsAction;

    [RelayCommand]
    public void SyncCharts()
    {
        ChartInformations = new ChartInformation();
        SyncChartsAction?.Invoke();
    }
}