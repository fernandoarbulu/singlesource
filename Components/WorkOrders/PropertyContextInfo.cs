namespace SinglesourceApp.Components.WorkOrders;

/// <summary>
/// Read-only property context shown in the Create Work Order modal.
/// Sourced from the parent page's already-loaded property DTO.
/// </summary>
public sealed class PropertyContextInfo
{
    public string Customer { get; set; } = "";

    public string LoanType { get; set; } = "";

    public string Investor { get; set; } = "";

    public string State { get; set; } = "";
}
