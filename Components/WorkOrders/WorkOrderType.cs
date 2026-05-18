namespace SinglesourceApp.Components.WorkOrders;

public sealed class WorkOrderType
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    /// <summary>
    /// Default line items loaded when this type is selected.
    /// Each item is cloned into the draft so edits don't mutate the catalog.
    /// </summary>
    public List<WorkOrderLineItem> DefaultItems { get; set; } = new();
}
