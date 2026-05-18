namespace SinglesourceApp.Components.WorkOrders;

/// <summary>
/// A single line item on a work order — either inherited from the work order type
/// configuration or manually added by the coordinator.
/// </summary>
public sealed class WorkOrderLineItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

    public string WorkItemName { get; set; } = "";

    public string Instructions { get; set; } = "";

    public string UoM { get; set; } = "EA";

    /// <summary>
    /// Client / AR billing rate. Loaded from work order type configuration.
    /// Always read-only in the UI — coordinators cannot edit the client rate.
    /// </summary>
    public decimal? AR { get; set; }

    /// <summary>
    /// Vendor / AP cost rate. Editable by the coordinator during work order creation.
    /// </summary>
    public decimal AP { get; set; }

    public decimal Qty { get; set; } = 1;

    /// <summary>Computed: AP × Qty. Never editable directly.</summary>
    public decimal LineTotal => Math.Round(AP * Qty, 2);

    /// <summary>
    /// True when this row originated from work order type defaults / pricing rules (vs. user-added supplements).
    /// UI uses this only for row labeling — removal policy is controlled by the hosting workflow (create modal allows removal).
    /// </summary>
    public bool IsInherited { get; set; }
}
