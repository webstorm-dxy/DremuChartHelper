using Avalonia.Controls;
using DremuChartHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DremuChartHelper.Views;

public partial class ResourceManagerView : UserControl
{
    public ResourceManagerView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider?.GetRequiredService<ResourceManagerViewModel>();
    }
}
