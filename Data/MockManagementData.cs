using SinglesourceApp.Models;

namespace SinglesourceApp.Data;

public static class MockManagementData
{
    public static List<ManagementRecord> Seed()
    {
        var today = DateTime.Today;

        return
        [
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                Name = "Northwind integration",
                Status = "Active",
                Category = "Operations",
                UpdatedAt = today.AddDays(-1),
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                Name = "Vendor onboarding",
                Status = "Active",
                Category = "Compliance",
                UpdatedAt = today.AddDays(-3),
            },
            new()
            {
                Id = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                Name = "Legacy data migration",
                Status = "Inactive",
                Category = "Operations",
                UpdatedAt = today.AddDays(-14),
            },
        ];
    }
}
