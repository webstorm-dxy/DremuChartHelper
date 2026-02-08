using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DremuChartHelper.Models.GorgeLinker.Repositories;
using DremuChartHelper.Models.GorgeLinker.Services;
using DremuChartHelper.Models.GorgeLinker.Filters;
using DremuChartHelper.Models.Repositories;
using DremuChartHelper.Models.Services;
using DremuChartHelper.Models;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Tests;

public class UnitTest1
{
    [Fact]
    public async Task 项目服务创建与验证()
    {
        var repository = new InMemoryProjectRepository();
        var service = new ProjectService(repository);

        var project = await service.CreateProjectAsync("Demo", "120", "c:\\demo.zip");
        var all = await service.GetAllProjectsAsync();

        Assert.Single(all);
        Assert.Equal(project.Path, all[0].Path);
    }

    [Fact]
    public async Task 图表服务与过滤器链执行()
    {
        var chartRepository = new InMemoryChartRepository();
        var chartService = new ChartDataService(chartRepository);
        var filter = new CaptureElementFilter();
        var filterManager = new FilterManager(chartRepository, chartService, new[] { filter });

        await chartService.EnsureInitializedAsync();
        await filterManager.ExecuteFiltersAsync();

        Assert.True(filter.ProcessedCount > 0);
    }
}

internal sealed class InMemoryProjectRepository : IProjectRepository
{
    private readonly List<ProjectInfo> _items = new();

    public Task<List<ProjectInfo>> LoadAllAsync()
    {
        return Task.FromResult(_items.ToList());
    }

    public Task SaveAllAsync(List<ProjectInfo> projects)
    {
        _items.Clear();
        _items.AddRange(projects);
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryChartRepository : IChartRepository
{
    public Task<ScoreInformation> GetScoreInformationAsync()
    {
        var info = new ScoreInformation
        {
            Staves =
            [
                new StaffInformation
                {
                    ClassName = "Demo",
                    DisplayName = "Demo",
                    Form = "Dremu",
                    Periods =
                    [
                        new PeriodInformation
                        {
                            MethodName = "P1",
                            TimeOffset = 0
                        }
                    ]
                }
            ]
        };
        return Task.FromResult(info);
    }

    public Task<ElementInformation[]> GetPeriodElementsAsync(string staffName, string periodName)
    {
        return Task.FromResult(new[]
        {
            new ElementInformation { ClassName = "Dremu.DremuMainLane" },
            new ElementInformation { ClassName = "Other" }
        });
    }
}

internal sealed class CaptureElementFilter : ElementFilterBase
{
    public int ProcessedCount { get; private set; }

    public override string Name => "Capture";

    public override bool ShouldProcess(ElementInformation element)
    {
        return element.ClassName == "Dremu.DremuMainLane";
    }

    public override Task ProcessElementsAsync(ElementInformation[] elements)
    {
        ProcessedCount += elements.Length;
        return Task.CompletedTask;
    }
}
