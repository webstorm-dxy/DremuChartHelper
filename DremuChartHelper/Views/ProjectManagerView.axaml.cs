using Avalonia.Controls;
using DremuChartHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DremuChartHelper.Views;

public partial class ProjectManagerView : UserControl
{
    public ProjectManagerView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider?.GetRequiredService<ProjectManagerViewModel>();
    }
}
