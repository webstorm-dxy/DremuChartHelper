using Avalonia.Controls;
using DremuChartHelper.ViewModels;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace DremuChartHelper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider?.GetRequiredService<MainWindowViewModel>();
        contentFrame.Navigate(typeof(ProjectManagerView));
    }
    
    private void OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.InvokedItemContainer is not NavigationViewItem { Tag: string tag }) return;
        switch (tag)
        {
            case "ProjectManager":
                contentFrame.Navigate(typeof(ProjectManagerView));
                break;
            case "CurveEditor":
                contentFrame.Navigate(typeof(CurveEditorView));
                break;
            case "ResourceManager":
                contentFrame.Navigate(typeof(ResourceManagerView));
                break;
        }
    }
}
