using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public class ReviewCard //TODO: Should become a propert of TaskInstance
    {
        public string ReviewType { get; set; } = "";
        public string ExplanationText { get; set; } = "";
        public string Status { get; set; } = "";
    }

    public partial class TaskSubmissionGuidanceCard : ComponentBase
    {
        [Parameter]
        public TaskInstance? TaskInstance { get; set; }

        private List<ReviewCard> Cards = new();

        protected override void OnInitialized()
        {
            Cards = new List<ReviewCard>
            {
                new ReviewCard
                {
                    ReviewType = "Client",
                    Status = "Required",
                    ExplanationText = GenerateExplanation("Client", "Required")
                },
                new ReviewCard
                {
                    ReviewType = "Investor",
                    Status = "Not Required",
                    ExplanationText = GenerateExplanation("Investor", "Not Required")
                },
            };
        }

        private string GenerateExplanation(string type, string status)
        {
            return $"{type} review is {status.ToLower()} — mock guidance text for demonstration purposes.";
        }
        private string GetBorderClass(string status)
        {
            return status switch
            {
                "Required" => "sg-border-error",
                "May Require" => "sg-border-warning",
                "Not Required" => "sg-border-info",
                _ => "sg-border-info"
            };
        }

        private KeyValuePair<string, string> GetChip(ReviewCard card)
        {
            return new KeyValuePair<string, string>(card.Status, GetTheme(card.Status));
        }

        private string GetTheme(string status) //TODO: Convert to telerik badge when review properties are implemented
        {
            return status switch
            {
                "Required" => "error",
                "May Require" => "warning",
                "Not Required" => "neutral",
                _ => "neutral"
            };
        }
    }
}