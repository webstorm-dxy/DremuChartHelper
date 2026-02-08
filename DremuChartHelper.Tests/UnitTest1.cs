using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
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

    [Fact]
    public async Task JSONRPC请求体_获取谱面方法正确()
    {
        var handler = new CaptureHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"jsonrpc\":\"2.0\",\"result\":{\"staves\":[]},\"id\":1}")
        });
        var httpClient = new HttpClient(handler);
        var repository = new GorgeStudioChartRepository("http://example.test", httpClient);

        await repository.GetScoreInformationAsync();

        var request = handler.Requests.Single();
        var body = request.Body;
        var json = JsonNode.Parse(body)!.AsObject();

        Assert.Equal("2.0", json["jsonrpc"]!.GetValue<string>());
        Assert.Equal("getScoreInformation", json["method"]!.GetValue<string>());
    }

    [Fact]
    public async Task JSONRPC请求体_获取周期元素参数正确()
    {
        var handler = new CaptureHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"jsonrpc\":\"2.0\",\"result\":[],\"id\":1}")
        });
        var httpClient = new HttpClient(handler);
        var repository = new GorgeStudioChartRepository("http://example.test", httpClient);

        await repository.GetPeriodElementsAsync("S1", "P1");

        var request = handler.Requests.Single();
        var body = request.Body;
        var json = JsonNode.Parse(body)!.AsObject();
        var parameters = json["params"]!.AsObject();

        Assert.Equal("getPeriodElements", json["method"]!.GetValue<string>());
        Assert.Equal("S1", parameters["staffName"]!.GetValue<string>());
        Assert.Equal("P1", parameters["periodName"]!.GetValue<string>());
    }

    [Fact]
    public void 数据模型_谱面序列化字段正确()
    {
        var model = new ScoreInformation
        {
            Staves =
            [
                new StaffInformation
                {
                    ClassName = "S",
                    DisplayName = "D",
                    Form = "F",
                    Periods =
                    [
                        new PeriodInformation
                        {
                            MethodName = "M",
                            TimeOffset = 1.5f
                        }
                    ]
                }
            ]
        };

        var json = JsonNode.Parse(JsonSerializer.Serialize(model))!.AsObject();
        var staff = json["staves"]!.AsArray()[0]!.AsObject();
        var period = staff["periods"]!.AsArray()[0]!.AsObject();

        Assert.Equal("S", staff["name"]!.GetValue<string>());
        Assert.Equal("D", staff["displayName"]!.GetValue<string>());
        Assert.Equal("F", staff["form"]!.GetValue<string>());
        Assert.Equal("M", period["name"]!.GetValue<string>());
        Assert.Equal(1.5f, period["timeOffset"]!.GetValue<float>());
    }

    [Fact]
    public void 数据模型_注入器字段序列化正确()
    {
        var element = new EditableElementInformation
        {
            ClassName = "C",
            DisplayName = "E",
            Type = "T",
            Fields =
            [
                new EditableInjectorFieldInformation
                {
                    FieldType = "F",
                    FieldName = "N",
                    DisplayName = "D",
                    Description = "Desc",
                    AllowDefault = true
                }
            ]
        };
        var injector = new EditableInjectorInformation
        {
            ClassName = "IC",
            DisplayName = "ID",
            Fields = element.Fields
        };

        var elementJson = JsonNode.Parse(JsonSerializer.Serialize(element))!.AsObject();
        var injectorJson = JsonNode.Parse(JsonSerializer.Serialize(injector))!.AsObject();

        Assert.Equal("C", elementJson["className"]!.GetValue<string>());
        Assert.Equal("E", elementJson["displayName"]!.GetValue<string>());
        Assert.Equal("T", elementJson["type"]!.GetValue<string>());
        Assert.Equal("F", elementJson["fields"]!.AsArray()[0]!.AsObject()["fieldType"]!.GetValue<string>());
        Assert.Equal("IC", injectorJson["className"]!.GetValue<string>());
        Assert.Equal("ID", injectorJson["displayName"]!.GetValue<string>());
    }

    [Fact]
    public void 数据模型_音符序列化字段正确()
    {
        var tap = JsonNode.Parse(JsonSerializer.Serialize(new TapNote
        {
            InsertIndex = 1,
            HitTime = 2,
            Position = 3
        }))!.AsObject();

        var hold = JsonNode.Parse(JsonSerializer.Serialize(new HoldNote
        {
            InsertIndex = 1,
            HitTime = 2,
            Position = 3,
            HoldTime = 4
        }))!.AsObject();

        var element = JsonNode.Parse(JsonSerializer.Serialize(new Element
        {
            InsertIndex = 1,
            InjectorCode = "X"
        }))!.AsObject();

        Assert.Equal(1, tap["insert_index"]!.GetValue<int>());
        Assert.Equal(2, tap["hit_time"]!.GetValue<float>());
        Assert.Equal(3, tap["position"]!.GetValue<float>());
        Assert.Equal(4, hold["hold_time"]!.GetValue<float>());
        Assert.Equal("X", element["injector_code"]!.GetValue<string>());
    }
}

internal sealed class CaptureHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public CaptureHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    public List<CapturedRequest> Requests { get; } = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content == null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method.Method, request.RequestUri?.ToString() ?? string.Empty, body));
        return _responder(request);
    }
}

internal sealed record CapturedRequest(string Method, string Url, string Body);

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
