using Avalonia.Controls;
using DremuChartHelper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DremuChartHelper.Views;

public partial class CurveEditorView : UserControl
{
    public CurveEditorView()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider?.GetRequiredService<CurveEditorViewModel>();
    }
}
