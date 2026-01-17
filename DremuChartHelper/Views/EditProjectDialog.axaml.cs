using Avalonia.Controls;
using DremuChartHelper.ViewModels;
using DremuChartHelper.Models;

namespace DremuChartHelper.Views;

public partial class EditProjectDialog : Window
{
    public ProjectInfo? Project { get; set; }

    public EditProjectDialog()
    {
        InitializeComponent();
    }

    protected override void OnOpened(System.EventArgs e)
    {
        base.OnOpened(e);
        // 在窗口打开时设置DataContext，传入项目信息
        DataContext = new EditProjectDialogViewModel(this, Project!);
    }
}