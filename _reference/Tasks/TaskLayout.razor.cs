using BlazorBusiness.Web.Components.Tasks.Grid;
using BlazorBusiness.Web.Models.Container;
using BlazorBusiness.Web.Models.Tasking;
using BlazorBusiness.Web.Services.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;

namespace BlazorBusiness.Web.Components.Tasks
{
    public partial class TaskLayout : ComponentBase
    {
        [Parameter]
        public List<TaskGridDisplayModel> AllTasks { get; set; } = new();

        [Parameter]
        public ComponentState State { get; set; } = ComponentState.Loading;

        [Parameter]
        public EventCallback<TaskActionClickedEventArgs> OnTaskActionClicked { get; set; }

        public bool MyTasksSelected = false;

        public List<TaskGridDisplayModel> MyTasks => AllTasks.Where(t => t.AssigneeEmployeeID.HasValue && t.AssigneeEmployeeID.Value == AuthenticationService.GetUserEmployeeID().Result).ToList();

        public void HandleTaskActionClicked(TaskActionClickedEventArgs args)
        {
            OnTaskActionClicked.InvokeAsync(args);
        }
    }
}
