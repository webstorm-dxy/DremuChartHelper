using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GorgeStudio.GorgeStudioServer;

namespace DremuChartHelper.Models.GorgeLinker.Repositories;

/// <summary>
/// GorgeStudio 图表数据仓储实现
/// 封装所有与 GorgeStudio 服务器的 JSON-RPC 通信细节
/// </summary>
public class GorgeStudioChartRepository : IChartRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serverUrl">GorgeStudio 服务器 URL，默认为 localhost:14324</param>
    public GorgeStudioChartRepository(string serverUrl = "http://localhost:14324", HttpClient? httpClient = null)
    {
        _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
        _httpClient = httpClient ?? new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<ScoreInformation> GetScoreInformationAsync()
    {
        return await CallJsonRpcAsync<ScoreInformation>("getScoreInformation");
    }

    public async Task<ElementInformation[]> GetPeriodElementsAsync(string staffName, string periodName)
    {
        return await CallJsonRpcAsync<ElementInformation[]>("getPeriodElements",
            new { staffName, periodName });
    }

    /// <summary>
    /// 调用 JSON-RPC 方法
    /// </summary>
    private async Task<T> CallJsonRpcAsync<T>(string method, object? parameters = null)
    {
        using var jsonContent = new StringContent(
            JsonSerializer.Serialize(new
            {
                jsonrpc = "2.0",
                method,
                @params = parameters,
                id = 1
            }),
            Encoding.UTF8,
            "application/json");

        // 添加 ConfigureAwait(false) 避免捕获同步上下文
        using var httpResponse = await _httpClient.PostAsync(_serverUrl, jsonContent)
            .ConfigureAwait(false);

        httpResponse.EnsureSuccessStatusCode();

        // 读取响应也不需要 UI 上下文
        var jsonResponse = await httpResponse.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        var rpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<T>>(jsonResponse);

        // 检查反序列化是否成功
        if (rpcResponse == null)
        {
            throw new Exception("Failed to deserialize JSON-RPC response");
        }

        // 检查是否有错误
        if (rpcResponse.Error != null)
        {
            throw new Exception($"JSON-RPC Error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
        }

        // 检查结果是否存在
        if (rpcResponse.Result == null)
        {
            throw new Exception("JSON-RPC response returned null result");
        }

        return rpcResponse.Result;
    }
}
