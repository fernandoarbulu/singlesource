using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;

namespace BlazorBusiness.Web.Components.Tasks.Badges
{
    public partial class StatusBadge : ComponentBase
    {
        [Parameter]
        public TaskInstanceStatus TaskInstanceStatus { get; set; }

        public string GetBadgeStyling(string status)
        {
            switch (status)
            {
                case "New":
                    return "status-badge status-badge--new";
                case "In Progress":
                    return "status-badge status-badge--in-progress";
                case "Cancelled":
                    return "status-badge status-badge--cancelled";
                case "Completed":
                    return "status-badge status-badge--completed";
                default:
                    return "status-badge";
            }
        }
    }
}