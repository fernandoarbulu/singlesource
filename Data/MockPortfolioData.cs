using SinglesourceApp.Models;

namespace SinglesourceApp.Data;

public static class MockPortfolioData
{
    public static IReadOnlyList<PortfolioProperty> All { get; } =
    [
        new()
        {
            Street = "123 Test Street",
            City = "Derry",
            State = "PA",
            Zip = "15627",
            LoanNumber = "LN-10004521",
            LoanRefNumber = "REF-883201",
            OrderNumber = "ORD-10492",
            CustomerName = "Shellpoint Mortgage Servicing",
        },
        new()
        {
            Street = "456 Oak Avenue",
            City = "Pittsburgh",
            State = "PA",
            Zip = "15213",
            LoanNumber = "LN-10004588",
            LoanRefNumber = "REF-883215",
            OrderNumber = "ORD-10501",
            CustomerName = "Midwest Loan Trust",
        },
        new()
        {
            Street = "789 River Road",
            City = "Erie",
            State = "PA",
            Zip = "16501",
            LoanNumber = "LN-10004602",
            LoanRefNumber = "REF-883240",
            OrderNumber = "ORD-10512",
            CustomerName = "Great Lakes Servicing",
        },
        new()
        {
            Street = "12 Maple Lane",
            City = "Harrisburg",
            State = "PA",
            Zip = "17101",
            LoanNumber = "LN-10004633",
            LoanRefNumber = "REF-883256",
            OrderNumber = "ORD-10520",
            CustomerName = "Valley National",
        },
        new()
        {
            Street = "900 Commerce Blvd",
            City = "Philadelphia",
            State = "PA",
            Zip = "19107",
            LoanNumber = "LN-10004677",
            LoanRefNumber = "REF-883301",
            OrderNumber = "ORD-10544",
            CustomerName = "Eastern Portfolio LLC",
        },
    ];
}
