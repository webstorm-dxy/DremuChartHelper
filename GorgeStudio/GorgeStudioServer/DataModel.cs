using System.Text.Json.Serialization;

namespace GorgeStudio.GorgeStudioServer;

public class DataModel
{
}

public class ScoreInformation
{
    [JsonPropertyName("staves")] public StaffInformation[] Staves { get; set; }
}

public class StaffInformation
{
    [JsonPropertyName("name")] public string ClassName { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("form")] public string Form { get; set; }

    [JsonPropertyName("periods")] public PeriodInformation[] Periods { get; set; }
}

public class PeriodInformation
{
    [JsonPropertyName("name")] public string MethodName { get; set; }

    [JsonPropertyName("timeOffset")] public float TimeOffset { get; set; }
}

public class ElementInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; }
}

public class EditableElementInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("fields")] public EditableInjectorFieldInformation[] Fields { get; set; }
}

public class EditableInjectorInformation
{
    [JsonPropertyName("className")] public string ClassName { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("fields")] public EditableInjectorFieldInformation[] Fields { get; set; }
}

public class EditableInjectorFieldInformation
{
    [JsonPropertyName("fieldType")] public string FieldType { get; set; }

    [JsonPropertyName("fieldName")] public string FieldName { get; set; }

    [JsonPropertyName("displayName")] public string DisplayName { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }

    [JsonPropertyName("allowDefault")] public bool AllowDefault { get; set; }
}

// 音符模型
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

    [JsonPropertyName("injector_code")] public string InjectorCode { get; set; }
}