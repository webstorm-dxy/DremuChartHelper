using Avalonia.Controls;
using DremuChartHelper.ViewModels;

namespace DremuChartHelper.Views;

public partial class ProjectManagerView : UserControl
{
    public ProjectManagerView()
    {
        InitializeComponent();
        DataContext = new ProjectManagerViewModel();
    }
}