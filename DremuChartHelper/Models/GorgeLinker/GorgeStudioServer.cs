using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GorgeStudio.GorgeStudioServer;

public class DataModel
{
}

public class ScoreInformation
{
    [JsonPropertyName("staves")] public StaffInformation[] Staves { get; set; } = Array.Empty<StaffInformation>();
}

public class StaffInformation
{
    [JsonPropertyName("name")] public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("form")] public string Form { get; set; } = string.Empty;

    [JsonPropertyName("periods")] public PeriodInformation[] Periods { get; set; } = Array.Empty<PeriodInformation>();
}

public class PeriodInformation
{
    [JsonPropertyName("name")] public string MethodName { get; set; } = string.Empty;

    [JsonPropertyName("timeOffset")] public float TimeOffset { get; set; }
}

public class ElementInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; } = string.Empty;
}

public class EditableElementInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;

    [JsonPropertyName("fields")] public EditableInjectorFieldInformation[] Fields { get; set; } = Array.Empty<EditableInjectorFieldInformation>();
}

public class EditableInjectorInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; } = string.Empty;

    [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("fields")] public EditableInjectorFieldInformation[] Fields { get; set; } = Array.Empty<EditableInjectorFieldInformation>();
}

public class EditableInjectorFieldInformation
{
    [JsonPropertyName("fieldType")] public string FieldType { get; set; } = string.Empty;

    [JsonPropertyName("fieldName")] public string FieldName { get; set; } = string.Empty;

    [JsonPropertyName("displayName")] public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;

    [JsonPropertyName("allowDefault")] public bool AllowDefault { get; set; }
}

public class TapNote
{
    [JsonPropertyName("insert_index")] public int InsertIndex { get; set; }

    [JsonPropertyName("hit_time")] public float HitTime { get; set; }

    [JsonPropertyName("position")] public float Position { get; set; }
}

public class TaplikNote
{
    [JsonPropertyName("insert_index")] public int InsertIndex { get; set; }

    [JsonPropertyName("hit_time")] public float HitTime { get; set; }

    [JsonPropertyName("position")] public float Position { get; set; }
}

public class DragNote
{
    [JsonPropertyName("insert_index")] public int InsertIndex { get; set; }

    [JsonPropertyName("hit_time")] public float HitTime { get; set; }

    [JsonPropertyName("position")] public float Position { get; set; }
}

public class HoldNote
{
    [JsonPropertyName("insert_index")] public int InsertIndex { get; set; }

    [JsonPropertyName("hit_time")] public float HitTime { get; set; }

    [JsonPropertyName("position")] public float Position { get; set; }

    [JsonPropertyName("hold_time")] public float HoldTime { get; set; }
}

public class Element
{
    [JsonPropertyName("insert_index")] public int InsertIndex { get; set; }

    [JsonPropertyName("injector_code")] public string InjectorCode { get; set; } = string.Empty;
}

public class RemoteFunction
{
    public static readonly Lazy<RemoteFunction> Instance = new(() => new RemoteFunction());

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
            JsonSerializer.Serialize(new { jsonrpc = "2.0", method, @params = parameters, id = 1 }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage httpResponse = await _httpClient.PostAsync(Url, jsonContent)
            .ConfigureAwait(false);

        httpResponse.EnsureSuccessStatusCode();

        var jsonResponse = await httpResponse.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        var rpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<T>>(jsonResponse);

        if (rpcResponse == null)
        {
            throw new Exception("Failed to deserialize JSON-RPC response");
        }

        if (rpcResponse.Error != null)
        {
            throw new Exception($"JSON-RPC Error {rpcResponse.Error.Code}: {rpcResponse.Error.Message}");
        }

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

    public async Task<ElementInformation[]> GetPeriodElements(string staffName, string periodName)
    {
        return await CallJsonRpcAsync<ElementInformation[]>("getPeriodElements", new { staffName, periodName });
    }
}

public class JsonRpcResponse<T>
{
    [JsonPropertyName("result")]
    public T? Result { get; set; }

    [JsonPropertyName("error")]
    public JsonRpcError? Error { get; set; }

    [JsonPropertyName("id")]
    public object? Id { get; set; }
}

public class JsonRpcError
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}
