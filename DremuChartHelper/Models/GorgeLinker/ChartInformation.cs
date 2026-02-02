using System;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.ViewModels;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker;

public class ChartInformation
{

    public string ErrorMessage { get; set; } = string.Empty;
    public bool IsLoading { get; set; } = true;

    public StaffInformation[]? Staves;

    public static readonly Lazy<ChartInformation> Instance = new(() => new ChartInformation());

    private Task? _initializationTask;

    private ChartInformation()
    {
        _initializationTask = InitializeAsync();
        MainWindowViewModel.SyncChartsAction += UpdateChartInformation;
    }

    public void UpdateChartInformation()
    {
        _initializationTask = InitializeAsync();
        GC.Collect();
    }

    public Task EnsureInitializedAsync()
    {
        return _initializationTask ?? Task.CompletedTask;
    }

    private async Task InitializeAsync()
    {
        try
        {
            var remoteFunction = new RemoteFunction();
            var scoreInformation = await remoteFunction.GetScoreInformationAsync();

            // 检查数据是否有效
            if (scoreInformation?.Staves == null || scoreInformation.Staves.Length == 0)
            {
                ErrorMessage = "没有可用的谱面数据";
                Console.WriteLine("警告: Staves 为空或 null");
                return;
            }

            // 在 UI 线程上处理数据
            Staves = scoreInformation.Staves;
            
            var dremuStarffNames = from information in Staves where information.Form == "Dremu" select information.ClassName;
            var dremuPeriods = from staffInformation in Staves where staffInformation.Form == "Dremu" select staffInformation.Periods;
            var elements = await remoteFunction.GetPeriodElements(dremuStarffNames.First(), dremuPeriods.First().First().MethodName);
            var tap = from elementInformation in elements
                where elementInformation.ClassName == "Dremu.DremuTap"
                select elementInformation;
            Console.Out.WriteLine($"{tap}");
            // TODO: 将数据绑定到属性供 View 使用
            // 例如：ScoreInformation = scoreInformation;
        }
        catch (Exception ex)
        {
            // 处理异常，可以显示错误信息给用户
            ErrorMessage = $"加载失败: {ex.Message}";
            Console.WriteLine($"加载失败: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}