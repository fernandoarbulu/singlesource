using System.Text.Json.Serialization;

namespace SinglesourceApp.Data;

public sealed class PropertyMdmRoot
{
    [JsonPropertyName("categories")]
    public List<string> Categories { get; set; } = [];

    [JsonPropertyName("fields")]
    public List<PropertyMdmFieldJson> Fields { get; set; } = [];
}

public sealed class PropertyMdmFieldJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("category")]
    public string Category { get; set; } = "";

    [JsonPropertyName("subsection")]
    public string Subsection { get; set; } = "";

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = "";

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = "";

    [JsonPropertyName("rawType")]
    public string RawType { get; set; } = "";

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public sealed record PropertyMdmSubsectionVm(
    string Key,
    string Title,
    IReadOnlyList<PropertyMdmFieldJson> Fields);

/// <summary>One top-level category in the property details inspector (ordered list; no health/status segmentation).</summary>
public sealed record PropertyMdmCategoryVm(
    string AnchorId,
    string Title,
    IReadOnlyList<PropertyMdmSubsectionVm> Subsections);
