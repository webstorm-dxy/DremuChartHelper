using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GorgeStudio.GorgeStudioServer;

public class RemoteFunction
{
    private readonly HttpClient _httpClient;

    private const string Url = "http://localhost:14324";

    public RemoteFunction()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    private async Task<T> CallJsonRpcAsync<T>(string method, object? parameters = null)
    {
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
                { jsonrpc = "2.0", method,parameters, id = 1 }),Encoding.UTF8,"application/json");

        // 添加 ConfigureAwait(false) 避免捕获同步上下文
        using HttpResponseMessage httpResponse = await _httpClient.PostAsync(Url, jsonContent)
            .ConfigureAwait(false);

        httpResponse.EnsureSuccessStatusCode();

        // 读取响应也不需要 UI 上下文
        var jsonResponse = await httpResponse.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        Console.WriteLine(jsonResponse);

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

    public async Task<ScoreInformation> GetScoreInformationAsync()
    {
        return await CallJsonRpcAsync<ScoreInformation>("getScoreInformation");
    }
}

public class JsonRpcResponse<T>
{
    [JsonPropertyName("result")]
    public T Result { get; set; }

    [JsonPropertyName("error")]
    public JsonRpcError Error { get; set; }

    [JsonPropertyName("id")]
    public object Id { get; set; }
}

public class JsonRpcError
{
    public int Code { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}