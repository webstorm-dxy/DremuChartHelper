using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DremuChartHelper.ViewModels;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Views;

public partial class CurveEditorView : UserControl
{
    public CurveEditorView()
    {
        InitializeComponent();
        DataContext = new CurveEditorViewModel();
    }
}