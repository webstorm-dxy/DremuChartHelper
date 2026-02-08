using Avalonia.Controls;
using DremuChartHelper.ViewModels;

namespace DremuChartHelper.Views;

public partial class CreateProjectDialog : Window
{
    public CreateProjectDialog()
    {
        InitializeComponent();
        DataContext = new CreateProjectDialogViewModel(this);
    }
}
