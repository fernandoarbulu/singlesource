namespace SinglesourceApp.Data;

/// <summary>Single field row.</summary>
public sealed record PropertyField(string Label, string Value, bool IsKeyMetric = false);

/// <summary>Logical group inside an expanded card (primary visible; secondary behind disclosure).</summary>
public sealed record PropertyFieldSection(
    string Name,
    IReadOnlyList<PropertyField> PrimaryFields,
    IReadOnlyList<PropertyField> SecondaryFields);

public sealed record PropertyCategory(
    string AnchorId,
    string Title,
    string IconSvg,
    string StatusLabel,
    bool IsDelayed,
    /// <summary>One-line human summary for problem cards, e.g. "Delayed — 90 days delinquent".</summary>
    string? ProblemSummaryLine,
    IReadOnlyList<PropertyField> SummaryFields,
    IReadOnlyList<PropertyFieldSection> ExpandedSections);

public static class PropertyOverviewData
{
    public static IReadOnlyList<PropertyCategory> Categories { get; } = Build();

    private static IReadOnlyList<PropertyCategory> Build()
    {
        var list = new List<PropertyCategory>();

        list.Add(Make(
            "pd-overview",
            "Overview",
            IconDoc,
            "Delayed",
            true,
            "Delayed — 90 days delinquent · UPB $250,000",
            new[]
            {
                new PropertyField("UPB", "$250,000", true),
                new PropertyField("Days Delinquent", "90", true)
            },
            OverviewSections()));

        list.Add(Make(
            "pd-inspections",
            "Inspections",
            IconInsp,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("Last inspection", "2025-10-15", true),
                new PropertyField("Next due", "2025-11-15", true)
            },
            InspectionSections()));

        list.Add(Make(
            "pd-structures",
            "Structures & Units",
            IconStruct,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("Units", "1", true),
                new PropertyField("Occupancy", "Vacant", true)
            },
            StructureSections()));

        list.Add(Make(
            "pd-site",
            "Site & Access",
            IconSite,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("Access verified", "Yes", true),
                new PropertyField("Lockbox", "LB-9921", true)
            },
            SiteSections()));

        list.Add(Make(
            "pd-maintenance",
            "Maintenance & Field Services",
            IconWrench,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("Open orders", "2", true),
                new PropertyField("Last service", "2025-09-01", true)
            },
            MaintenanceSections()));

        list.Add(Make(
            "pd-conveyance",
            "Conveyance & HUD",
            IconHud,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("HUD case", "HUD-44102", true),
                new PropertyField("Stage", "Pre-conveyance", true)
            },
            ConveyanceSections()));

        list.Add(Make(
            "pd-escalations",
            "Escalations",
            IconEsc,
            "Delayed",
            true,
            "Delayed — 90 days delinquent · investor review",
            new[]
            {
                new PropertyField("UPB", "$250,000", true),
                new PropertyField("Days Delinquent", "90", true)
            },
            EscalationSections()));

        list.Add(Make(
            "pd-violations",
            "Violations & Compliance",
            IconShield,
            "No Issues",
            false,
            null,
            new[]
            {
                new PropertyField("Open violations", "0", true),
                new PropertyField("Last municipal check", "2025-08-01", true)
            },
            ViolationSections()));

        list.Add(Make(
            "pd-workflow",
            "Workflow & Oversight",
            IconFlow,
            "Delayed",
            true,
            "Delayed — 90 days delinquent · workflow hold",
            new[]
            {
                new PropertyField("UPB", "$250,000", true),
                new PropertyField("Days Delinquent", "90", true)
            },
            WorkflowSections()));

        return list;
    }

    private static PropertyCategory Make(
        string anchorId,
        string title,
        string iconSvg,
        string status,
        bool delayed,
        string? problemLine,
        PropertyField[] summary,
        List<PropertyFieldSection> sections) =>
        new(anchorId, title, iconSvg, status, delayed, problemLine, summary, sections);

    private static List<PropertyFieldSection> OverviewSections() =>
    [
        new("Financial",
            new[]
            {
                new PropertyField("UPB", "$250,000", true),
                new PropertyField("Escrow balance", "$1,240", true),
                new PropertyField("Reserve held", "$500", false)
            },
            Secondary("Financial", 6)),
        new("Loan",
            new[]
            {
                new PropertyField("Loan type", "FHA", true),
                new PropertyField("Interest rate", "6.25%", true),
                new PropertyField("Maturity date", "2035-04-01", false)
            },
            Secondary("Loan", 5)),
        new("Timeline",
            new[]
            {
                new PropertyField("Boarding date", "2024-01-05", true),
                new PropertyField("Last payment", "2024-07-12", true),
                new PropertyField("Days delinquent", "90", true)
            },
            Secondary("Timeline", 5)),
        new("References",
            new[]
            {
                new PropertyField("Investor loan ID", "INV-882901", false),
                new PropertyField("Custodian", "Core", false)
            },
            Secondary("References", 4))
    ];

    private static List<PropertyFieldSection> InspectionSections() =>
    [
        new("Scheduling",
            new[]
            {
                new PropertyField("Request source", "System generated", true),
                new PropertyField("Frequency", "Monthly", false)
            },
            Secondary("Scheduling", 4)),
        new("Results",
            new[]
            {
                new PropertyField("Last inspection complete", "2025-10-15", true),
                new PropertyField("Condition score", "B", true)
            },
            Secondary("Results", 5)),
        new("History",
            new[]
            {
                new PropertyField("Inspections YTD", "10", false),
                new PropertyField("Failed inspections", "0", false)
            },
            Secondary("History", 3))
    ];

    private static List<PropertyFieldSection> StructureSections() =>
    [
        new("Structure",
            new[]
            {
                new PropertyField("First known vacancy", "2025-07-01", true),
                new PropertyField("Year built", "1985", false),
                new PropertyField("Sq ft", "1,420", false)
            },
            Secondary("Structure", 5)),
        new("Units",
            new[]
            {
                new PropertyField("Most recent occupancy", "Vacant", true),
                new PropertyField("Unit count", "1", true)
            },
            Secondary("Units", 3)),
        new("Safety",
            new[]
            {
                new PropertyField("Utilities on", "No", false),
                new PropertyField("Winterization", "Complete", false)
            },
            Secondary("Safety", 4))
    ];

    private static List<PropertyFieldSection> SiteSections() =>
    [
        new("Access",
            new[]
            {
                new PropertyField("Gate code on file", "Yes", true),
                new PropertyField("HOA contact", "On file", false)
            },
            Secondary("Access", 4)),
        new("Lot",
            new[]
            {
                new PropertyField("Lot size", "0.12 ac", false),
                new PropertyField("Zoning", "R-1", false)
            },
            Secondary("Lot", 3)),
        new("Hazards",
            new[]
            {
                new PropertyField("Flood zone", "X", false),
                new PropertyField("Pool", "No", false)
            },
            Secondary("Hazards", 3))
    ];

    private static List<PropertyFieldSection> MaintenanceSections() =>
    [
        new("Field services",
            new[]
            {
                new PropertyField("Grass cut cycle", "Bi-weekly", true),
                new PropertyField("Debris last cleared", "2025-09-28", true)
            },
            Secondary("Field services", 5)),
        new("Vendors",
            new[]
            {
                new PropertyField("Primary vendor", "ABC Services", false),
                new PropertyField("Snow removal", "N/A", false)
            },
            Secondary("Vendors", 3)),
        new("Equipment",
            new[]
            {
                new PropertyField("Lockbox", "LB-9921", true),
                new PropertyField("Alarm", "Not armed", false)
            },
            Secondary("Equipment", 3))
    ];

    private static List<PropertyFieldSection> ConveyanceSections() =>
    [
        new("HUD",
            new[]
            {
                new PropertyField("HUD case #", "44102", true),
                new PropertyField("Conveyance deadline", "2025-12-01", true)
            },
            Secondary("HUD", 4)),
        new("Title",
            new[]
            {
                new PropertyField("Title status", "Clear", false),
                new PropertyField("Liens", "0 reported", false)
            },
            Secondary("Title", 3)),
        new("Disposition",
            new[]
            {
                new PropertyField("Marketing status", "Not listed", false),
                new PropertyField("List price", "—", false)
            },
            Secondary("Disposition", 3))
    ];

    private static List<PropertyFieldSection> EscalationSections() =>
    [
        new("Financial",
            new[]
            {
                new PropertyField("UPB", "$250,000", true),
                new PropertyField("Arrears", "$12,400", true),
                new PropertyField("Days delinquent", "90", true)
            },
            Secondary("Escalation financial", 4)),
        new("Investor",
            new[]
            {
                new PropertyField("Investor", "VWH", true),
                new PropertyField("Review queue", "Tier 2", true)
            },
            Secondary("Investor", 3)),
        new("Actions",
            new[]
            {
                new PropertyField("Last touch", "2025-10-01", false),
                new PropertyField("Next review", "2025-11-01", false)
            },
            Secondary("Actions", 3))
    ];

    private static List<PropertyFieldSection> ViolationSections() =>
    [
        new("Compliance",
            new[]
            {
                new PropertyField("Municipal registration", "Current", true),
                new PropertyField("Open violations", "0", true)
            },
            Secondary("Compliance", 4)),
        new("Codes",
            new[]
            {
                new PropertyField("Last code check", "2025-08-01", false),
                new PropertyField("HOA fines", "None", false)
            },
            Secondary("Codes", 3)),
        new("Insurance",
            new[]
            {
                new PropertyField("Hazard policy", "Active", false),
                new PropertyField("Liability", "Active", false)
            },
            Secondary("Insurance", 3))
    ];

    private static List<PropertyFieldSection> WorkflowSections() =>
    [
        new("Oversight",
            new[]
            {
                new PropertyField("Workflow owner", "Field Ops", true),
                new PropertyField("Days delinquent", "90", true)
            },
            Secondary("Oversight", 4)),
        new("SLA",
            new[]
            {
                new PropertyField("SLA target", "48h", false),
                new PropertyField("Breached", "Yes", true)
            },
            Secondary("SLA", 3)),
        new("Audit",
            new[]
            {
                new PropertyField("Last audit", "2025-09-15", false),
                new PropertyField("Findings", "2 minor", false)
            },
            Secondary("Audit", 3))
    ];

    private static List<PropertyField> Secondary(string prefix, int count)
    {
        var rows = new List<PropertyField>(count);
        for (var i = 1; i <= count; i++)
        {
            rows.Add(new PropertyField(
                $"{prefix} · detail {i}",
                i % 3 == 0 ? "Pending" : $"Value {i:D3}",
                false));
        }

        return rows;
    }

    private const string IconDoc = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><path d="M14 2v6h6"/><path d="M8 13h8M8 17h8M8 9h4"/></svg>""";
    private const string IconInsp = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M9 11l3 3L22 4"/><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/></svg>""";
    private const string IconStruct = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18M5 21V7l7-4 7 4v14"/><path d="M9 21v-6h6v6"/></svg>""";
    private const string IconSite = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><path d="M2 12h20"/></svg>""";
    private const string IconWrench = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/></svg>""";
    private const string IconHud = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M9 21V9"/></svg>""";
    private const string IconEsc = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><path d="M12 9v4M12 17h.01"/></svg>""";
    private const string IconShield = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>""";
    private const string IconFlow = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="3"/><path d="M12 2v4M12 18v4M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M2 12h4M18 12h4M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83"/></svg>""";
}
