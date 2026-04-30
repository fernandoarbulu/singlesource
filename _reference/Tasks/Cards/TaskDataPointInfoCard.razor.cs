using BlazorBusiness.Web.Models.Tasking;
using Microsoft.AspNetCore.Components;
using BlazorBusiness.Web.Models.Container;
using SingleSource.WebAPI.Business.SDK;
using Microsoft.IdentityModel.Tokens;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskDataPointInfoCard : ComponentBase
    {
        [Parameter]
        public List<KeyValuePair<string, string>> WorkOrderDetailLayoutFields { get; set; }
        [Parameter]
        public string ReferenceNumber { get; set; }

        public ComponentState State { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (WorkOrderDetailLayoutFields is not null)
            {
                State = ComponentState.Content;
            }
        }

        protected async override Task OnInitializedAsync()
        {
            State = ComponentState.Content;         
        }
    }
}
