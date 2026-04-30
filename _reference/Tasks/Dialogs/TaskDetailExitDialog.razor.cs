using Microsoft.AspNetCore.Components;

namespace BlazorBusiness.Web.Components.Tasks.Dialogs
{
    public partial class TaskDetailExitDialog
    {
        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback VisibleChanged { get; set; }

        private async Task OnVisibleChanged(bool currVisible)
        {
            Visible = currVisible;
            await VisibleChanged.InvokeAsync();
        }

        private async Task OnSave()
        {
            Visible = false;
            await VisibleChanged.InvokeAsync();
            //TODO 
        }

        private void OnDiscard()
        {
            Nav.NavigateTo($"/tasks/queue");
        }
    }
}
