using SinglesourceApp.Data;
using SinglesourceApp.Models;

namespace SinglesourceApp.Services;

/// <summary>Maps between API-shaped DTOs and the existing MDM view-model pipeline (<see cref="PropertyMdmCatalog"/>).</summary>
public static class PropertyDetailsDtoMapper
{
    public static PropertyFieldDto ToFieldDto(PropertyMdmFieldJson f) =>
        new()
        {
            Id = f.Id,
            Label = f.Label,
            Category = f.Category,
            Subsection = f.Subsection,
            Visibility = f.Visibility,
            Kind = f.Kind,
            RawType = f.RawType,
            Value = f.Value
        };

    public static PropertyMdmFieldJson ToFieldJson(PropertyFieldDto f) =>
        new()
        {
            Id = f.Id,
            Label = f.Label,
            Category = f.Category,
            Subsection = f.Subsection,
            Visibility = f.Visibility,
            Kind = f.Kind,
            RawType = f.RawType,
            Value = f.Value
        };

    /// <summary>Flattens sections back into a single <see cref="PropertyMdmRoot"/> for <see cref="PropertyMdmCatalog.BuildCategories"/>.</summary>
    public static PropertyMdmRoot ToMdmRoot(PropertyDetailsDto dto)
    {
        var fields = new List<PropertyMdmFieldJson>();
        foreach (var section in dto.Sections)
        foreach (var f in section.Fields)
            fields.Add(ToFieldJson(f));

        return new PropertyMdmRoot { Fields = fields };
    }

    /// <summary>Builds DTO sections + hero summary from a loaded MDM root (same grouping rules as the catalog).</summary>
    public static PropertyDetailsDto FromMdmRoot(
        PropertyMdmRoot root,
        string addressLine1,
        string cityStateZip,
        string heroStatusLabel,
        string heroStatusKind,
        WorkOrdersSummaryDto workOrders)
    {
        var fieldIndex = root.Fields.Select((f, i) => (f, i)).ToDictionary(x => x.f.Id, x => x.i, StringComparer.OrdinalIgnoreCase);

        var dto = new PropertyDetailsDto
        {
            AddressLine1 = addressLine1,
            CityStateZip = cityStateZip,
            HeroStatusLabel = heroStatusLabel,
            HeroStatusKind = heroStatusKind,
            WorkOrdersSummary = workOrders
        };

        foreach (var cat in PropertyMdmCatalog.DeriveCategoryOrder(root))
        {
            var catFields = root.Fields
                .Where(f => string.Equals(f.Category, cat, StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => fieldIndex.GetValueOrDefault(f.Id, int.MaxValue))
                .ToList();
            if (catFields.Count == 0)
                continue;

            dto.Sections.Add(new PropertySectionDto
            {
                AnchorId = PropertyMdmCatalog.AnchorFor(cat),
                Title = cat,
                Fields = catFields.Select(ToFieldDto).ToList()
            });
        }

        dto.HeroSummaryFields = BuildHeroSummaryFields(root);
        return dto;
    }

    /// <summary>Ordered hero metrics; always includes every slot so the hero strip stays aligned.</summary>
    private static List<PropertyFieldDto> BuildHeroSummaryFields(PropertyMdmRoot root)
    {
        var fields = root.Fields ?? [];
        string? V(string label) =>
            fields.FirstOrDefault(f => string.Equals(f.Label, label, StringComparison.OrdinalIgnoreCase))?.Value;

        // Display labels match the existing hero strip; values resolve from MDM field labels.
        var specs = new (string DisplayLabel, string MdmLabel)[]
        {
            ("Loan status", "Loan Status"),
            ("Occupancy", "Most Recent Occupancy Status"),
            ("Days delinquent", "Days Delinquent"),
            ("Loan type", "Loan Type"),
            ("UPB", "UPB"),
            ("Last inspection", "Last Inspection Complete Date"),
            ("APN", "APN"),
            ("Investor", "Investor")
        };

        var list = new List<PropertyFieldDto>(specs.Length);
        var i = 0;
        foreach (var (displayLabel, mdmLabel) in specs)
        {
            var src = fields.FirstOrDefault(f => string.Equals(f.Label, mdmLabel, StringComparison.OrdinalIgnoreCase));
            var value = V(mdmLabel);
            list.Add(new PropertyFieldDto
            {
                Id = src?.Id ?? $"hero-{i}",
                Label = displayLabel,
                Category = src?.Category ?? "",
                Subsection = src?.Subsection ?? "",
                Visibility = src?.Visibility ?? "default",
                Kind = src?.Kind ?? "text",
                RawType = src?.RawType ?? "",
                Value = value
            });
            i++;
        }

        return list;
    }
}
