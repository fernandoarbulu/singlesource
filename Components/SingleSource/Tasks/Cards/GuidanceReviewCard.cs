namespace BlazorBusiness.Web.Components.Tasks.Cards;

/// <summary>
/// Display model for a single review entry in <see cref="TaskSubmissionGuidanceCard"/>.
/// Ported from reference ReviewCard. Removed TaskInstance dependency.
/// TODO: align with backend TaskInstance review properties when API is connected.
/// </summary>
public sealed class GuidanceReviewCard
{
    public string ReviewType      { get; set; } = "";
    public string ExplanationText { get; set; } = "";
    public string Status          { get; set; } = "";
}
