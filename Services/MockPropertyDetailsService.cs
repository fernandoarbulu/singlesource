// This mock represents a realistic sample property payload used to validate UI rendering and will be replaced by a real API response.

using System.Text.Json;
using SinglesourceApp.Data;
using SinglesourceApp.Models;

namespace SinglesourceApp.Services;

/// <summary>
/// In-memory stand-in for the real property-details API. Returns one fully populated <see cref="PropertyDetailsDto"/>
/// built from the embedded MDM catalog plus realistic sample values for a single property.
/// Swap for <c>HttpPropertyDetailsService</c> (or similar) when the backend is ready — keep the same DTO contract.
/// </summary>
public sealed class MockPropertyDetailsService : IPropertyDetailsService
{
    private const string SampleAddressLine1 = "123 Main St";
    private const string SampleCityStateZip = "Los Angeles, CA 90001";

    private static readonly JsonSerializerOptions CloneOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>Replaces catalog &quot;Sample value NNN&quot; placeholders by label. Value <c>null</c> = empty (UI shows —).</summary>
    private static readonly Dictionary<string, string?> SamplePlaceholderByLabel = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Prior HUD reimbursable Non-P and P"] = "$310",
        ["Listing Agent Name"] = "Maria Garcia",
        ["Post Sale Update"] = "Weekly file update — 03/10/2025",
        ["Re Disposition"] = "Deferred — title review",
        ["Guarantor Case Number"] = null,
        ["New Loan Servicer"] = "Mr. Cooper Loan Servicing",
        ["Servicer Loan Number"] = "SLN-8849201",
        ["Servicer Name"] = "LoanCare, LLC",
        ["Hybrid or Non-Hybrid"] = "Hybrid",
        ["Requestor Code"] = "REQ-LA-WEST-01",
        ["Number of Units"] = "1",
        ["Dehumidifier"] = "Portable — garage",
        ["Sump Pump"] = "Submersible — operational",
        ["Lock Box Location"] = "Front porch handrail",
        ["Padlock Code"] = "9921",
        ["HOA Maintained"] = "Yes — common areas",
        ["Pool"] = "In-ground, screened",
        ["Exclude from Lawn Replan"] = "No",
        ["Lot Size SqFt."] = "6,200",
        ["Paused Replan"] = "No",
        ["BNMP_Date"] = "02/14/2025",
        ["BSO Passed"] = "Yes",
        ["BCMP_Date"] = "03/01/2025",
        ["Second Chance Closing Approved"] = "Yes",
        ["FHA VA Stop Work"] = "No",
        ["Property Reconveyance"] = "Pending investor approval",
        ["Exception"] = null,
        ["Weather Exception"] = null,
        ["Violation Items"] = "Tall grass; peeling paint (front elevation)",
        ["Days Off DNI File"] = "12",
        ["Order File Name"] = "ORD-2025-0142-main",
        ["Hazard Claim #"] = "HC-2024-99103",
        ["Insurance Responsibility"] = "Servicer",
        ["VPR De-registration required"] = "No",
        ["VPR Registered Agent"] = "Cogency Global Inc.",
        ["Lock Box Code"] = "4587"
    };

    /// <inheritdoc />
    public Task<PropertyDetailsDto> GetPropertyDetailsAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var root = CloneRoot(PropertyMdmCatalog.LoadRoot());
        ApplySamplePropertyValues(root);
        ReplaceSamplePlaceholders(root.Fields);

        var workOrders = new WorkOrdersSummaryDto
        {
            Open = 3,
            Billed = 2,
            Cancelled = 1
        };

        var dto = PropertyDetailsDtoMapper.FromMdmRoot(
            root,
            addressLine1: SampleAddressLine1,
            cityStateZip: SampleCityStateZip,
            heroStatusLabel: "Active",
            heroStatusKind: "success",
            workOrders: workOrders);

        return Task.FromResult(dto);
    }

    /// <summary>
    /// Sets <see cref="PropertyMdmFieldJson.Value"/> by label match. Use <c>null</c> for empty (UI shows —); do not use "N/A".
    /// </summary>
    private static void SetFieldValue(IReadOnlyList<PropertyMdmFieldJson> fields, string label, string? value)
    {
        foreach (var f in fields)
        {
            if (string.Equals(f.Label, label, StringComparison.OrdinalIgnoreCase))
            {
                f.Value = value;
                return;
            }
        }
    }

    private static void ApplySamplePropertyValues(PropertyMdmRoot root)
    {
        var fields = root.Fields;

        // Eviction & Occupancy
        SetFieldValue(fields, "Eviction Complete Date", null);
        SetFieldValue(fields, "Eviction Start Date", null);
        SetFieldValue(fields, "Personal Property over $500", "No");
        SetFieldValue(fields, "Personal Property Present", "Yes");
        SetFieldValue(fields, "PPE Expiration Date", null);

        // Financials (Overview / Financials subsection)
        SetFieldValue(fields, "Prior P&P", "$806");
        SetFieldValue(fields, "Total Spent Client", "$393");
        SetFieldValue(fields, "Prior HUD reimbursable Non-P&P", "$422");
        SetFieldValue(fields, "Prior Non-HUD reimbursable", "$113");
        SetFieldValue(fields, "Total Expenses", "$1,734");
        SetFieldValue(fields, "EOB Amount", null);
        SetFieldValue(fields, "EOB Received Date", null);

        // Hero + loan summary (Overview)
        SetFieldValue(fields, "Loan Status", "Active");
        SetFieldValue(fields, "Loan Type", "Conventional");
        SetFieldValue(fields, "APN", "555-123-456");
        SetFieldValue(fields, "Most Recent Occupancy Status", "Occupied");
        SetFieldValue(fields, "UPB", "$245,300");
        SetFieldValue(fields, "Investor", "Fannie Mae");
        SetFieldValue(fields, "Days Delinquent", "32");

        // Inspections
        SetFieldValue(fields, "Last Inspection Complete Date", "03/12/2025");
        SetFieldValue(fields, "Inspection Type", "Completed");
        SetFieldValue(fields, "Inspection Request Source", "John Martinez");
        SetFieldValue(fields, "Inspections Not Allowed", "No");

        // Site & Access — Lock Code stands in for lockbox code; Gate Code empty
        SetFieldValue(fields, "Gate Code", null);
        SetFieldValue(fields, "Lock Code", "4587");
    }

    /// <summary>Any field whose value still starts with &quot;Sample value&quot; gets a realistic value or null (— in UI).</summary>
    private static void ReplaceSamplePlaceholders(IReadOnlyList<PropertyMdmFieldJson> fields)
    {
        foreach (var f in fields)
        {
            var v = f.Value;
            if (string.IsNullOrEmpty(v) || !v.StartsWith("Sample value", StringComparison.OrdinalIgnoreCase))
                continue;
            if (SamplePlaceholderByLabel.TryGetValue(f.Label, out var replacement))
                f.Value = replacement;
            else
                f.Value = null;
        }
    }

    private static PropertyMdmRoot CloneRoot(PropertyMdmRoot source)
    {
        var json = JsonSerializer.Serialize(source, CloneOpts);
        return JsonSerializer.Deserialize<PropertyMdmRoot>(json, CloneOpts)
               ?? throw new InvalidOperationException("Property MDM clone failed.");
    }
}
