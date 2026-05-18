namespace SinglesourceApp.Components.WorkOrders;

/// <summary>
/// Snapshot of the Create Work Order form at the point of save.
/// Passed to the parent component via <c>OnSave</c> for persistence.
/// </summary>
public sealed class WorkOrderDraft
{
    public string SelectedTypeName { get; set; } = "";

    public List<WorkOrderLineItem> LineItems { get; set; } = new();

    public DateTime? ClientDueDate { get; set; }

    public DateTime? VendorDueDate { get; set; }

    public string? VendorInstructions { get; set; }

    public string? InternalNotes { get; set; }
}
