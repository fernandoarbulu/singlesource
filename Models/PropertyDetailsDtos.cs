namespace SinglesourceApp.Models;

/// <summary>Top-level payload for GET /properties/{id}/details — mirrors the intended backend contract.</summary>
public sealed class PropertyDetailsDto
{
    public string AddressLine1 { get; set; } = "";

    public string CityStateZip { get; set; } = "";

    /// <summary>Status label for the hero badge (sentence case, e.g. &quot;Active&quot;).</summary>
    public string HeroStatusLabel { get; set; } = "";

    /// <summary>Maps to <c>StatusBadgeVariant</c> names: Active, Inactive, Pending, Neutral.</summary>
    public string HeroStatusKind { get; set; } = "Active";

    public WorkOrdersSummaryDto WorkOrdersSummary { get; set; } = new();

    /// <summary>Ordered categories (left nav); fields retain <see cref="PropertyFieldDto.Subsection"/> for inspector grouping.</summary>
    public List<PropertySectionDto> Sections { get; set; } = new();

    /// <summary>
    /// Hero strip metrics (fixed display order on the client).
    /// When populated, the page prefers this list; otherwise it derives values from <see cref="Sections"/> by label.
    /// </summary>
    public List<PropertyFieldDto> HeroSummaryFields { get; set; } = new();
}

/// <summary>Work order counts shown in the hero aside.</summary>
public sealed class WorkOrdersSummaryDto
{
    public int Open { get; set; }

    public int Billed { get; set; }

    public int Cancelled { get; set; }
}

/// <summary>One navigable category (left nav) containing all fields for that category.</summary>
public sealed class PropertySectionDto
{
    public string AnchorId { get; set; } = "";

    public string Title { get; set; } = "";

    public List<PropertyFieldDto> Fields { get; set; } = new();
}

/// <summary>Single MDM-style field row (inspector + optional hero reuse).</summary>
public sealed class PropertyFieldDto
{
    public string Id { get; set; } = "";

    public string Label { get; set; } = "";

    public string Category { get; set; } = "";

    public string Subsection { get; set; } = "";

    public string Visibility { get; set; } = "default";

    public string Kind { get; set; } = "";

    public string RawType { get; set; } = "";

    /// <summary>Null or empty displays as &quot;—&quot; in the UI.</summary>
    public string? Value { get; set; }
}
