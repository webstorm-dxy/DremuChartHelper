using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DremuChartHelper.ViewModels;

namespace DremuChartHelper.Controls;

public partial class TrackView : UserControl
{
    public TrackView()
    {
        InitializeComponent();

        // 设置默认的 ViewModel
        if (DataContext is null)
        {
            DataContext = new TimelineViewModel();
        }
    }
}