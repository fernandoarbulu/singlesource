using BlazorBusiness.Web.Components.SingleSourceGrid;

namespace SinglesourceApp.Models;

// COMPONENT: PropertyFinancialsTab
//
// BACKEND API CONTRACT — FinancialGridRow
//   This class is the grid row model for the Property Financials tab.
//   Map your API financial-line-item response DTO to this class when connecting the backend.
//
//   Each row = one AP or AR line item tied to a work order.
//   WorkOrder is the Telerik grouping key — rows with the same value are grouped together.
//   The InitialGroupDescriptors in PropertyFinancialsTab references WorkOrder by name; do not rename.
//
//   Nullable decimal fields (ApQty, VendorFee, ArQty, ArUnit, ClientFee) should be null
//   when not applicable for that row (e.g. a pure AP row has no AR fields).
//
//   Status fields accept free-form strings; the component maps them to CSS pill classes.
//   See StatusPillClass() in PropertyFinancialsTab.razor for the full set of recognized values.

/// <summary>
/// Display model for a single row in the Property Financials grid.
/// Attributes on this class drive SingleSourceGrid column generation via reflection.
/// </summary>
[SingleSourceGridGroup(DisplayName = "Payable",    Key = "Payable",    Order = 7, Collapsible = true)]
[SingleSourceGridGroup(DisplayName = "Receivable", Key = "Receivable", Order = 8, Collapsible = true)]
[SingleSourceGridGroup(DisplayName = "Status",     Key = "Status",     Order = 9, Collapsible = true)]
public class FinancialGridRow
{
    // Hidden from visible columns; used only as the Telerik InitialGroupDescriptors grouping field.
    [SingleSourceGridAttribute(AutoGenerate = false)]
    public string WorkOrder { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "AP GPID",      Width = "120px", Order = 1, Groupable = false)]
    public string ApGpid { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "AR GPID",      Width = "120px", Order = 2, Groupable = false)]
    public string ArGpid { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "Line Item",    Width = "150px", Order = 3, Groupable = false)]
    public string LineItem { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "Description",  Width = "210px", Order = 4)]
    public string Description { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "Imported Date", Width = "125px", Order = 5, Groupable = false)]
    public DateTime ImportedDate { get; set; }

    [SingleSourceGridAttribute(DisplayName = "Vendor",       Width = "175px", Order = 6)]
    public string Vendor { get; set; } = "";

    // ── Payable group ─────────────────────────────────────────────────────────

    [SingleSourceGridAttribute(DisplayName = "QTY",     Width = "65px",  GroupKey = "Payable", Order = 1, Groupable = false)]
    public decimal? ApQty { get; set; }

    [SingleSourceGridAttribute(DisplayName = "UNIT",    Width = "65px",  GroupKey = "Payable", Order = 2, Groupable = false)]
    public string? ApUnit { get; set; }

    [SingleSourceGridAttribute(DisplayName = "Vendor $", Width = "110px", GroupKey = "Payable", Order = 3, Groupable = false)]
    public decimal? VendorFee { get; set; }

    // ── Receivable group ──────────────────────────────────────────────────────

    [SingleSourceGridAttribute(DisplayName = "QTY",     Width = "65px",  GroupKey = "Receivable", Order = 1, Groupable = false)]
    public decimal? ArQty { get; set; }

    [SingleSourceGridAttribute(DisplayName = "UNIT",    Width = "65px",  GroupKey = "Receivable", Order = 2, Groupable = false)]
    public string? ArUnit { get; set; }

    [SingleSourceGridAttribute(DisplayName = "Client $", Width = "110px", GroupKey = "Receivable", Order = 3, Groupable = false)]
    public decimal? ClientFee { get; set; }

    // ── Status group ──────────────────────────────────────────────────────────

    [SingleSourceGridAttribute(DisplayName = "Business", Width = "165px", GroupKey = "Status", Order = 1, Groupable = false)]
    public string BusinessStatus { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "Client",   Width = "195px", GroupKey = "Status", Order = 2, Groupable = false)]
    public string ClientStatus { get; set; } = "";

    [SingleSourceGridAttribute(DisplayName = "Investor", Width = "205px", GroupKey = "Status", Order = 3, Groupable = false)]
    public string InvestorStatus { get; set; } = "";
}
