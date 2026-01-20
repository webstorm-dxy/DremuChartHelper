using System;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.ViewModels;

public partial class CurveEditorViewModel : ViewModelBase
{
    

    // 构造函数保持简单，不进行异步操作
    public CurveEditorViewModel()
    {
        MainWindowViewModel.SyncChartsAction += () =>
        {
            Console.WriteLine("test");
        };
    }

    
}