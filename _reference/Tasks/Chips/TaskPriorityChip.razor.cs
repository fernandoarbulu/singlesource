using BlazorBusiness.Web.Models.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Chips
{
    public partial class TaskPriorityChip : ComponentBase
    {
        [Parameter]
        public TaskPriority TaskInstancePriority { get; set; }
        [Parameter]
        public EventCallback ChipClickedCallback { get; set; }
        [Parameter]
        public string Class { get; set; }

        public KeyValuePair<string, string> TextAndColorPair { get; set; }

        protected override void OnInitialized()
        {
            TextAndColorPair = CalculateThemeColorBasedOnPriorityValue();
        }

        public async Task ChipClickEventHandler()
        {
            await ChipClickedCallback.InvokeAsync();
        }

        public string GetChipStyling(KeyValuePair<string, string> kvp)
        {
            switch (kvp.Value)
            {
                case "error":
                    return "red-chip";
                case "warning":
                    return "yellow-chip";
                case "base":
                    return "grey-chip";
                default:
                    return "";
            }
        }

        public KeyValuePair<string, string> CalculateThemeColorBasedOnPriorityValue()
        {
            switch (TaskInstancePriority.ToString().ToLower().Trim())
            {
                case "high":
                    return new KeyValuePair<string, string>(TaskInstancePriority.ToString(), "error");
                case "medium":
                    return new KeyValuePair<string, string>(TaskInstancePriority.ToString(), "warning");
                case "low":
                    return new KeyValuePair<string, string>(TaskInstancePriority.ToString(), "base");
                default:
                    return new KeyValuePair<string, string>(TaskInstancePriority.ToString(), "none");

            }
        }

    }
}
