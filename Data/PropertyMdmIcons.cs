namespace SinglesourceApp.Data;

/// <summary>Category icons for Property MDM navigation (same glyphs as prior property overview).</summary>
public static class PropertyMdmIcons
{
    public const string Overview = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><path d="M14 2v6h6"/><path d="M8 13h8M8 17h8M8 9h4"/></svg>""";
    public const string Inspections = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M9 11l3 3L22 4"/><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"/></svg>""";
    public const string StructuresUnits = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M3 21h18M5 21V7l7-4 7 4v14"/><path d="M9 21v-6h6v6"/></svg>""";
    public const string SiteAccess = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="10"/><path d="M2 12h20"/></svg>""";
    public const string Maintenance = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/></svg>""";
    public const string ConveyanceHud = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M9 21V9"/></svg>""";
    public const string Escalations = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><path d="M12 9v4M12 17h.01"/></svg>""";
    public const string Violations = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>""";
    public const string Workflow = """<svg class="pd-svg-ico" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5"><circle cx="12" cy="12" r="3"/><path d="M12 2v4M12 18v4M4.93 4.93l2.83 2.83M16.24 16.24l2.83 2.83M2 12h4M18 12h4M4.93 19.07l2.83-2.83M16.24 7.76l2.83-2.83"/></svg>""";

    public static string ForCategory(string title) => title switch
    {
        "Overview" => Overview,
        "Inspections" => Inspections,
        "Structures & Units" => StructuresUnits,
        "Site & Access" => SiteAccess,
        "Maintenance & Field Services" => Maintenance,
        "Conveyance & HUD" => ConveyanceHud,
        "Escalations" => Escalations,
        "Violations & Compliance" => Violations,
        "Workflow & Oversight" => Workflow,
        _ => Overview
    };
}
