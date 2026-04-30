using BlazorBusiness.Web.Models.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Grid
{
    public partial class TaskGrid
    {

        [Parameter]
        public List<TaskGridDisplayModel> TasksToDisplay { get; set; } = new();


        [Parameter]
        public EventCallback<TaskActionClickedEventArgs> OnTaskActionClicked { get; set; }

        protected int CurrentUserID { get; set; }

        [Parameter]
        public EventCallback<TaskGridDisplayModel> RowOrChipClicked { get; set; }

        public string GetDueDateCssClass(TaskGridDisplayModel task)
        {
            string status = task.TaskInstanceStatusName?.Trim() ?? string.Empty;
            bool isCompletedOrCancelled =
                status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);

            if (isCompletedOrCancelled)
            {
                return string.Empty;
            }

            DateTime today = DateTime.Today;
            DateTime dueDate = task.DueDate.Date;

            if (today > dueDate)
            {
                return "due-date-overdue";
            }

            if (today == dueDate)
            {
                return "due-date-today";
            }

            return string.Empty;
        }

        private bool IsCompletedOrCancelled(TaskGridDisplayModel task)
        {
            return string.Equals(task.TaskInstanceStatusName, "Completed", StringComparison.OrdinalIgnoreCase)
                || string.Equals(task.TaskInstanceStatusName, "Cancelled", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAssigned(TaskGridDisplayModel task)
        {
            return task.AssigneeEmployeeID.HasValue;
        }

        private bool IsAssignedToMe(TaskGridDisplayModel task)
        {
            return IsAssigned(task)
                && task.AssigneeEmployeeID == CurrentUserID;
        }

        private bool IsAssignedToSomeoneElse(TaskGridDisplayModel task)
        {
            return IsAssigned(task)
                && task.AssigneeEmployeeID != CurrentUserID;
        }

        private bool IsViewAction(TaskGridDisplayModel task)
        {
            return IsCompletedOrCancelled(task) || IsAssignedToSomeoneElse(task);
        }

        public string GetActionText(TaskGridDisplayModel task)
        {
            if (IsViewAction(task))
            {
                return "View";
            }

            if (IsAssignedToMe(task))
            {
                return "Resume";
            }

            return "Start";
        }

        public void OnActionClicked(TaskGridDisplayModel task)
        {
            string actionText = GetActionText(task);

            new TaskActionClickedEventArgs
            {
                TaskInstanceID = task.TaskInstanceID,
                Action = actionText
            };

            OnTaskActionClicked.InvokeAsync(new TaskActionClickedEventArgs
            {
                TaskInstanceID = task.TaskInstanceID,
                Action = actionText
            });
        }

        protected string GetDueDateText(TaskGridDisplayModel task)
        {
            if (task.DueDate == default)
            {
                return "NA";
            }

            return task.DueDate.ToString("MM/dd/yyyy");
        }

        protected string GetStatusClass(string status)
        {
            return status?.ToLower() switch
            {
                "new" => "status-badge status-badge--new",
                "in progress" => "status-badge status-badge--in-progress",
                "cancelled" => "status-badge status-badge--cancelled",
                "completed" => "status-badge status-badge--done",
                _ => "status-badge"
            };
        }

        protected string GetRank(TaskGridDisplayModel task)
        {
            return (TasksToDisplay.IndexOf(task)  + 1).ToString();
        }

        public string GetTaskTypeString(object context)
        {
            var gridModel = context as TaskGridDisplayModel;
            return gridModel.TaskTemplateName;

        }

        public async Task OnRowClickHandler(GridRowClickEventArgs args)
        {
            var model = args.Item as TaskGridDisplayModel;
            await this.RowOrChipClicked.InvokeAsync(model);
        }        

        public async Task HandleEditedRecords(IEnumerable<TaskGridDisplayModel> editedRecords)
        {

        }
    }
}