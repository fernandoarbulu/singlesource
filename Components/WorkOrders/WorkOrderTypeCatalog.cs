namespace SinglesourceApp.Components.WorkOrders;

/// <summary>
/// Static catalog of work order types and their default line items.
/// Replace with an API-backed service once the data layer is wired.
/// </summary>
public static class WorkOrderTypeCatalog
{
    public static readonly IReadOnlyList<WorkOrderType> Types = BuildTypes();

    /// <summary>
    /// Supplemental work item names available for manual add-on (not type defaults).
    /// These appear in the "Add Work Item" dropdown after a type is selected.
    /// </summary>
    public static readonly IReadOnlyList<string> AllWorkItemNames =
    [
        "Boarding",
        "Lock Change",
        "Rekey Locks",
        "Board Windows",
        "Trash Out",
        "Appliance Removal",
        "HVAC Inspection",
        "Pool Cleanup",
        "Pest Treatment",
    ];

    private sealed record SupplementalTemplate(string Instructions, string UoM, decimal? AR, decimal AP);

    private static readonly Dictionary<string, SupplementalTemplate> SupplementalTemplates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Boarding"] = new(
            "Board accessible openings per investor boarding spec; photograph completed barriers.",
            "Sq Ft", 2.25m, 1.65m),
        ["Lock Change"] = new(
            "Replace exterior locksets and verify keys; deliver copies per coordinator instructions.",
            "EA", 55m, 40m),
        ["Rekey Locks"] = new(
            "Rekey cylinders to a single master; confirm all exterior doors operate smoothly.",
            "EA", 35m, 25m),
        ["Board Windows"] = new(
            "Install temporary plywood covers on specified openings; fasten per safety guidelines.",
            "Sq Ft", 2.25m, 1.65m),
        ["Trash Out"] = new(
            "Remove baggable debris from designated rooms; stage for haul-off.",
            "LS", 95m, 70m),
        ["Appliance Removal"] = new(
            "Disconnect and remove listed appliances; leave utilities capped safely.",
            "EA", 85m, 65m),
        ["HVAC Inspection"] = new(
            "Visual inspection of HVAC components; note leaks, damage, and filter condition.",
            "LS", 75m, 55m),
        ["Pool Cleanup"] = new(
            "Skim surface debris, verify pump access, and document visible safety concerns.",
            "LS", 120m, 90m),
        ["Pest Treatment"] = new(
            "Treat designated interior perimeter per label; note occupied-unit precautions.",
            "LS", 145m, 110m),
    };

    /// <summary>
    /// Builds a supplemental line item seeded from <see cref="AllWorkItemNames"/> templates (falls back to generic defaults).
    /// Always returns <see cref="WorkOrderLineItem.IsInherited"/> false — user-added supplements for the create-work-order flow.
    /// </summary>
    public static WorkOrderLineItem CreateSupplementalLine(string name)
    {
        var key = name.Trim();
        if (!SupplementalTemplates.TryGetValue(key, out var tmpl))
        {
            return new WorkOrderLineItem
            {
                WorkItemName   = key,
                Instructions   = "Coordinate scope and pricing with vendor before dispatch.",
                UoM            = "EA",
                AR             = null,
                AP             = 0m,
                Qty            = 1,
                IsInherited    = false,
            };
        }

        return new WorkOrderLineItem
        {
            WorkItemName   = key,
            Instructions   = tmpl.Instructions,
            UoM            = tmpl.UoM,
            AR             = tmpl.AR,
            AP             = tmpl.AP,
            Qty            = 1,
            IsInherited    = false,
        };
    }

    private static List<WorkOrderType> BuildTypes() =>
    [
        new WorkOrderType
        {
            Id = "winterization",
            Name = "Winterization",
            DefaultItems =
            [
                Item("Winterize Plumbing",  "LS",      AR: 85m,  AP: 65m,  qty: 1,
                     "Shut off water at the main and drain all accessible supply lines, fixtures, and water heater."),
                Item("Pressure Test",       "EA",      AR: 45m,  AP: 35m,  qty: 1,
                     "Pressure-test supply lines after draining to confirm no active leaks."),
                Item("Antifreeze Application", "EA",   AR: 35m,  AP: 26m,  qty: 1,
                     "Apply RV-grade antifreeze to toilet traps, floor drains, and all P-traps."),
            ]
        },
        new WorkOrderType
        {
            Id = "initial-secure",
            Name = "Initial Secure",
            DefaultItems =
            [
                Item("Dead Bolt Installation", "EA",   AR: 55m,  AP: 40m,  qty: 1,
                     "Install keyed dead bolt on all exterior access doors and verify function."),
                Item("Window Board-Up",     "Sq Ft",   AR: 2.25m, AP: 1.65m, qty: 10,
                     "Cover broken or unsecured windows with 3/8 in. plywood per investor spec."),
                Item("Emergency Lock Change", "EA",    AR: 45m,  AP: 35m,  qty: 1,
                     "Change exterior door locks and deliver two key copies to the coordinator."),
            ]
        },
        new WorkOrderType
        {
            Id = "lawn-maintenance",
            Name = "Lawn Maintenance",
            DefaultItems =
            [
                Item("Front Lawn Cut",      "Sq Ft",   AR: 75m,  AP: 55m,  qty: 1,
                     "Mow front lawn to 3 in. height; bag and remove clippings."),
                Item("Rear Yard Cleanup",   "Sq Ft",   AR: 65m,  AP: 50m,  qty: 1,
                     "Mow and clear rear yard; remove loose debris and bag clippings."),
                Item("Edge & Trim",         "HR",      AR: 40m,  AP: 30m,  qty: 1,
                     "Edge along walkways, driveways, and curbs; trim overgrowth at fence lines."),
            ]
        },
        new WorkOrderType
        {
            Id = "debris-removal",
            Name = "Debris Removal",
            DefaultItems =
            [
                Item("Interior Debris Removal", "Cubic Yd", AR: 95m, AP: 70m, qty: 1,
                     "Remove all personal property, trash, and debris from interior rooms."),
                Item("Exterior Haul Away",  "Load",    AR: 120m, AP: 90m,  qty: 1,
                     "Load and haul exterior debris to nearest licensed disposal facility."),
                Item("Disposal Fee",        "LS",      AR: 45m,  AP: 35m,  qty: 1,
                     "Landfill tipping fee charged per load at disposal site."),
            ]
        },
        new WorkOrderType
        {
            Id = "lock-change",
            Name = "Lock Change",
            DefaultItems =
            [
                Item("Front Door Lock Change", "EA",   AR: 55m,  AP: 40m,  qty: 1,
                     "Replace front door lockset and deadbolt; provide 3 key copies."),
                Item("Rear Door Lock Change",  "EA",   AR: 55m,  AP: 40m,  qty: 1,
                     "Replace rear or side exterior door lockset."),
                Item("Rekey All Exterior Locks", "EA", AR: 35m,  AP: 25m,  qty: 3,
                     "Rekey all exterior locks to a single master key for access consistency."),
            ]
        },
        new WorkOrderType
        {
            Id = "title-search",
            Name = "Title Search",
            DefaultItems =
            [
                Item("Full Title Search",   "LS",      AR: 150m, AP: 120m, qty: 1,
                     "Pull current title report from county records to identify liens and encumbrances."),
                Item("Tax Lien Search",     "LS",      AR: 75m,  AP: 60m,  qty: 1,
                     "Search county tax records for delinquent taxes and pending assessments."),
            ]
        },
        new WorkOrderType
        {
            Id = "property-inspection",
            Name = "Property Inspection",
            DefaultItems =
            [
                Item("Exterior Inspection", "LS",      AR: 65m,  AP: 50m,  qty: 1,
                     "Photograph all exterior elevations; document visible damage and access points."),
                Item("Occupancy Verification", "LS",   AR: 55m,  AP: 45m,  qty: 1,
                     "Confirm occupancy status via visual inspection and neighbor canvass if needed."),
                Item("Damage Documentation", "LS",     AR: 35m,  AP: 25m,  qty: 1,
                     "Provide GPS-tagged interior and exterior photo report with condition summary."),
            ]
        },
        new WorkOrderType
        {
            Id = "boarding",
            Name = "Boarding",
            DefaultItems =
            [
                Item("Window Boarding",     "Sq Ft",   AR: 2.25m, AP: 1.65m, qty: 10,
                     "Cover all accessible windows with 3/8 in. plywood per GSE boarding spec."),
                Item("Door Boarding",       "EA",      AR: 85m,  AP: 65m,  qty: 1,
                     "Secure all non-primary access doors with plywood barrier and fasteners."),
                Item("Garage Door Boarding", "EA",     AR: 125m, AP: 95m,  qty: 1,
                     "Board or secure garage door opening per investor guidelines."),
            ]
        },
    ];

    /// <summary>
    /// Clones a type's default items so edits in the draft don't mutate the catalog.
    /// Returns an empty list when <see cref="WorkOrderType.DefaultItems"/> is empty (e.g. bid-driven types with no pricing-rule rows yet).
    /// </summary>
    public static List<WorkOrderLineItem> CloneDefaultItems(WorkOrderType type) =>
        type.DefaultItems
            .Select(src => new WorkOrderLineItem
            {
                WorkItemName = src.WorkItemName,
                Instructions = src.Instructions,
                UoM          = src.UoM,
                AR           = src.AR,
                AP           = src.AP,
                Qty          = src.Qty,
                IsInherited  = src.IsInherited,
            })
            .ToList();

    private static WorkOrderLineItem Item(
        string name, string uom, decimal AR, decimal AP, decimal qty, string instructions) => new()
    {
        WorkItemName = name,
        Instructions = instructions,
        UoM          = uom,
        AR           = AR,
        AP           = AP,
        Qty          = qty,
        IsInherited  = true,
    };
}
