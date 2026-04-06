namespace SinglesourceApp.Models;

public sealed class ManagementRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
