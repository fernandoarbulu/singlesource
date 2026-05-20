using BlazorBusiness.Web.Components.SingleSourceGrid;
using Microsoft.AspNetCore.Components;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Grid
{
    public partial class TaskGrid
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public List<TaskGridDisplayModel> TasksToDisplay { get; set; } = new();

        [Parameter]
        public EventCallback<TaskActionClickedEventArgs> OnTaskActionClicked { get; set; }

        [Parameter]
        public EventCallback<TaskGridDisplayModel> RowOrChipClicked { get; set; }

        protected int CurrentUserID { get; set; }

        public string GetDueDateCssClass(TaskGridDisplayModel task)
        {
            string status = task.TaskInstanceStatusName?.Trim() ?? string.Empty;
            bool isCompletedOrCancelled =
                status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
                status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);

            if (isCompletedOrCancelled)
                return string.Empty;

            DateTime today = DateTime.Today;
            DateTime dueDate = task.DueDate.Date;

            if (today > dueDate)
                return "due-date-overdue";

            if (today == dueDate)
                return "due-date-today";

            return string.Empty;
        }

        private bool IsCompletedOrCancelled(TaskGridDisplayModel task) =>
            string.Equals(task.TaskInstanceStatusName, "Completed", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(task.TaskInstanceStatusName, "Cancelled", StringComparison.OrdinalIgnoreCase);

        private bool IsAssigned(TaskGridDisplayModel task) =>
            task.AssigneeEmployeeID.HasValue;

        private bool IsAssignedToMe(TaskGridDisplayModel task) =>
            IsAssigned(task) && task.AssigneeEmployeeID == CurrentUserID;

        private bool IsAssignedToSomeoneElse(TaskGridDisplayModel task) =>
            IsAssigned(task) && task.AssigneeEmployeeID != CurrentUserID;

        private bool IsViewAction(TaskGridDisplayModel task) =>
            IsCompletedOrCancelled(task) || IsAssignedToSomeoneElse(task);

        public string GetActionText(TaskGridDisplayModel task)
        {
            if (IsViewAction(task))   return "View";
            if (IsAssignedToMe(task)) return "Resume";
            return "Start";
        }

        public void OnActionClicked(TaskGridDisplayModel task)
        {
            string action = GetActionText(task);

            if (action is "Start" or "Resume")
            {
                Navigation.NavigateTo($"/invoice-review-task/{task.TaskInstanceID}");
                return;
            }

            OnTaskActionClicked.InvokeAsync(new TaskActionClickedEventArgs
            {
                TaskInstanceID = task.TaskInstanceID,
                Action         = action
            });
        }

        protected string GetDueDateText(TaskGridDisplayModel task)
        {
            if (task.DueDate == default)
                return "NA";

            return task.DueDate.ToString("MM/dd/yyyy");
        }

        protected string GetStatusClass(string? status) =>
            status?.ToLower() switch
            {
                "new"         => "ss-pill ss-pill--corrections",
                "in progress" => "ss-pill ss-pill--pending",
                "cancelled"   => "ss-pill ss-pill--neutral",
                "completed"   => "ss-pill ss-pill--approved",
                _             => "ss-pill ss-pill--neutral"
            };

        protected string GetRank(TaskGridDisplayModel task) =>
            (TasksToDisplay.IndexOf(task) + 1).ToString();

        public async Task OnRowClickHandler(GridRowClickEventArgs args)
        {
            if (args.Item is TaskGridDisplayModel model)
                await RowOrChipClicked.InvokeAsync(model);
        }
    }

    /// <summary>
    /// Local display model for <see cref="TaskGrid"/>.
    /// Properties are decorated with <see cref="SingleSourceGridAttribute"/> so
    /// <c>SingleSourceGrid&lt;TItem&gt;</c> can auto-generate the column layout.
    /// </summary>
    public sealed class TaskGridDisplayModel
    {
        [SingleSourceGridAttribute(AutoGenerate = false)]
        public int TaskInstanceID { get; set; }

        [SingleSourceGridAttribute(DisplayName = "#", Width = "52px", Order = 1,
            Sortable = false, Filterable = false, Groupable = false)]
        public int Ranking { get; set; }

        [SingleSourceGridAttribute(DisplayName = "Task Type", Width = "180px", Order = 2)]
        public string TaskTemplateName { get; set; } = string.Empty;

        [SingleSourceGridAttribute(DisplayName = "Address", Width = "220px", Order = 3)]
        public string PropertyAddress { get; set; } = string.Empty;

        [SingleSourceGridAttribute(DisplayName = "Due Date", Width = "110px", Order = 4,
            Groupable = false)]
        public DateTime DueDate { get; set; }

        [SingleSourceGridAttribute(DisplayName = "Status", Width = "140px", Order = 5)]
        public string? TaskInstanceStatusName { get; set; }

        [SingleSourceGridAttribute(AutoGenerate = false)]
        public int? AssigneeEmployeeID { get; set; }

        [SingleSourceGridAttribute(DisplayName = "Action", Width = "110px", Order = 6,
            Sortable = false, Filterable = false, Groupable = false, Editable = false)]
        public string? Action { get; set; }
    }
}
