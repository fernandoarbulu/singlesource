namespace SinglesourceApp.Models;

/// <summary>Row for Portfolio Search results (mock).</summary>
public sealed class PortfolioProperty
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

    public string LoanNumber { get; set; } = string.Empty;

    public string LoanRefNumber { get; set; } = string.Empty;

    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string AddressLine2 => $"{City}, {State} {Zip}";
}
