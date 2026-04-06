using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SinglesourceApp.Data;

public static class PropertyMdmCatalog
{
    /// <summary>Display order when the JSON "categories" array is missing or empty.</summary>
    public static readonly string[] CategoryOrder =
    [
        "Overview",
        "Inspections",
        "Structures & Units",
        "Site & Access",
        "Maintenance & Field Services",
        "Conveyance & HUD",
        "Escalations",
        "Violations & Compliance",
        "Workflow & Oversight"
    ];

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static PropertyMdmRoot? _cachedRoot;

    /// <summary>
    /// Loads the MDM schema from an embedded resource (no HTTP). Cached for the app lifetime.
    /// </summary>
    public static PropertyMdmRoot LoadRoot()
    {
        if (_cachedRoot is not null)
            return _cachedRoot;

        var assembly = typeof(PropertyMdmCatalog).Assembly;
        var stream = OpenEmbeddedJsonStream(assembly);
        if (stream is null)
        {
            var all = string.Join(", ", assembly.GetManifestResourceNames());
            throw new InvalidOperationException(
                "Embedded MDM JSON not found. Expected PropertyMdmV4.json (see csproj LogicalName). " +
                $"Manifest resources: [{all}]");
        }

        string json;
        using (var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("Embedded MDM JSON is empty.");

        var root = JsonSerializer.Deserialize<PropertyMdmRoot>(json, JsonOpts);
        if (root is null)
            throw new InvalidOperationException("Property MDM JSON deserialized to null.");

        root.Fields ??= [];

        if (root.Fields.Count == 0)
            throw new InvalidOperationException("Property MDM JSON has no fields.");

        _cachedRoot = root;
        return _cachedRoot;
    }

    private static Stream? OpenEmbeddedJsonStream(Assembly assembly)
    {
        // 1) Stable name from csproj LogicalName
        foreach (var name in new[] { "PropertyMdmV4.json", "SinglesourceApp.Data.PropertyMdmV4.json" })
        {
            var s = assembly.GetManifestResourceStream(name);
            if (s is not null)
                return s;
        }

        // 2) Fallback: any resource ending with PropertyMdmV4.json
        var names = assembly.GetManifestResourceNames();
        var match = names.FirstOrDefault(static n =>
            n.EndsWith("PropertyMdmV4.json", StringComparison.Ordinal));
        match ??= names.FirstOrDefault(static n =>
            n.Contains("PropertyMdmV4", StringComparison.OrdinalIgnoreCase));
        return match is null ? null : assembly.GetManifestResourceStream(match);
    }

    /// <summary>
    /// Category order for the UI: PDF order first, then any extra categories present on fields.
    /// Does not rely on the JSON "categories" array matching field strings (avoids empty UI when they drift).
    /// </summary>
    public static IReadOnlyList<string> DeriveCategoryOrder(PropertyMdmRoot root)
    {
        var fields = root.Fields;
        if (fields.Count == 0)
            return [];

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<string>();
        foreach (var c in CategoryOrder)
        {
            if (!fields.Any(f => string.Equals(f.Category, c, StringComparison.OrdinalIgnoreCase)))
                continue;
            ordered.Add(c);
            seen.Add(c);
        }

        foreach (var f in fields)
        {
            if (string.IsNullOrWhiteSpace(f.Category))
                continue;
            if (seen.Add(f.Category))
                ordered.Add(f.Category);
        }

        return ordered;
    }

    public static IReadOnlyList<PropertyMdmCategoryVm> BuildCategories(PropertyMdmRoot root)
    {
        var list = new List<PropertyMdmCategoryVm>();
        var fieldIndex = root.Fields.Select((f, i) => (f, i)).ToDictionary(x => x.f.Id, x => x.i, StringComparer.OrdinalIgnoreCase);

        foreach (var cat in DeriveCategoryOrder(root))
        {
            var fields = root.Fields.Where(f => string.Equals(f.Category, cat, StringComparison.OrdinalIgnoreCase)).ToList();
            if (fields.Count == 0)
                continue;

            var subGroups = fields
                .GroupBy(f => f.Subsection, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Min(f => fieldIndex.GetValueOrDefault(f.Id, int.MaxValue)))
                .Select(g => new PropertyMdmSubsectionVm(
                    SubsectionKey(cat, g.Key),
                    SubsectionDisplayTitle(g.Key),
                    g.OrderBy(f => fieldIndex.GetValueOrDefault(f.Id, int.MaxValue)).ToList()))
                .ToList();

            var anchor = AnchorFor(cat);
            list.Add(new PropertyMdmCategoryVm(anchor, cat, subGroups));
        }

        return list;
    }

    /// <summary>Maps schema subsection keys to display titles. Unknown/placeholder keys must never show as &quot;?&quot;.</summary>
    private static string SubsectionDisplayTitle(string? subsection)
    {
        if (string.IsNullOrWhiteSpace(subsection))
            return "General";
        var t = subsection.Trim();
        if (t == "?")
            return "General";
        return t;
    }

    public static string AnchorFor(string category) =>
        category switch
        {
            "Overview" => "pd-overview",
            "Inspections" => "pd-inspections",
            "Structures & Units" => "pd-structures",
            "Site & Access" => "pd-site",
            "Maintenance & Field Services" => "pd-maintenance",
            "Conveyance & HUD" => "pd-conveyance",
            "Escalations" => "pd-escalations",
            "Violations & Compliance" => "pd-violations",
            "Workflow & Oversight" => "pd-workflow",
            _ => "pd-" + category.ToLowerInvariant().Replace(' ', '-').Replace("&", "").Replace("--", "-")
        };

    private static string SubsectionKey(string category, string subsection)
    {
        var slug = string.IsNullOrEmpty(subsection)
            ? "general"
            : new string(subsection.Select(c => char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-').ToArray());
        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        return $"{AnchorFor(category)}--{slug}";
    }
}
