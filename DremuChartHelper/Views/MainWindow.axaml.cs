using System;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace DremuChartHelper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
        }
    }
}