namespace BlazorBusiness.Web.Components.Tasks.Cards;

/// <summary>
/// Local display model for a file attached to a task.
/// Replaces the SDK's <c>OrderLinkedFile</c> model.
/// TODO: align with backend OrderLinkedFile when API is connected.
/// </summary>
public sealed class TaskDocumentFile
{
    public string FileName    { get; set; } = "";
    public string? FileUrl    { get; set; }
    public int?   OrderDetailId { get; set; }
}
