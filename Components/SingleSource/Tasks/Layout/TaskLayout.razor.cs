using BlazorBusiness.Web.Components.Tasks.Grid;
using Microsoft.AspNetCore.Components;

namespace BlazorBusiness.Web.Components.Tasks.Layout
{
    public partial class TaskLayout
    {
        // ── Display parameters ────────────────────────────────────────────

        /// <summary>Title / template name shown in the task header bar.</summary>
        [Parameter] public string TaskTitle { get; set; } = "Task";

        /// <summary>Optional one-line description shown below the title.</summary>
        [Parameter] public string? TaskDescription { get; set; }

        /// <summary>Human-readable status label (e.g. "In Progress", "New").</summary>
        [Parameter] public string TaskStatus { get; set; } = "New";

        /// <summary>Optional due date shown in the header. Omit to hide the field.</summary>
        [Parameter] public DateTime? TaskDueDate { get; set; }

        // ── Guidance panel ────────────────────────────────────────────────

        /// <summary>
        /// When <c>true</c> the collapsible guidance panel is rendered.
        /// Defaults to <c>false</c>; set to <c>true</c> if you supply <see cref="GuidanceContent"/>.
        /// </summary>
        [Parameter] public bool ShowGuidance { get; set; }

        /// <summary>Optional rich content rendered inside the guidance panel.</summary>
        [Parameter] public RenderFragment? GuidanceContent { get; set; }

        // ── Grid data ─────────────────────────────────────────────────────

        /// <summary>
        /// Task rows to display. When <c>null</c> or empty the component falls back to
        /// <see cref="MockTasks"/> so the layout is always visually populated.
        /// </summary>
        [Parameter] public List<TaskGridDisplayModel>? Tasks { get; set; }

        // ── Events ────────────────────────────────────────────────────────

        [Parameter] public EventCallback<TaskActionClickedEventArgs> OnTaskActionClicked { get; set; }
        [Parameter] public EventCallback<TaskGridDisplayModel>       OnRowClicked         { get; set; }

        // ── Internal state ────────────────────────────────────────────────

        private bool _guidanceExpanded = true;

        private void ToggleGuidance() => _guidanceExpanded = !_guidanceExpanded;

        // ── Derived display helpers ───────────────────────────────────────

        /// <summary>CSS modifier key derived from <see cref="TaskStatus"/>.</summary>
        private string StatusCssKey => TaskStatus?.ToLower() switch
        {
            "new"         => "new",
            "in progress" => "in-progress",
            "completed"   => "completed",
            "cancelled"   => "cancelled",
            _             => "default"
        };

        /// <summary>CSS class applied to the due-date label.</summary>
        private string DueDateCssClass
        {
            get
            {
                if (!TaskDueDate.HasValue) return string.Empty;
                if (DateTime.Today > TaskDueDate.Value.Date) return "tl-due--overdue";
                if (DateTime.Today == TaskDueDate.Value.Date) return "tl-due--today";
                return string.Empty;
            }
        }

        /// <summary>Rows passed to <see cref="TaskGrid"/>; falls back to mock data when empty.</summary>
        private List<TaskGridDisplayModel> EffectiveTasks =>
            Tasks is { Count: > 0 } ? Tasks : MockTasks;

        // ── Mock data ─────────────────────────────────────────────────────

        private static readonly List<TaskGridDisplayModel> MockTasks =
        [
            new()
            {
                TaskInstanceID         = 1,
                TaskTemplateName       = "Property Inspection",
                PropertyAddress        = "123 Main St, Springfield, IL 62701",
                DueDate                = DateTime.Today.AddDays(3),
                TaskInstanceStatusName = "In Progress",
                AssigneeEmployeeID     = null,
                Action                 = null
            },
            new()
            {
                TaskInstanceID         = 2,
                TaskTemplateName       = "Title Review",
                PropertyAddress        = "456 Oak Ave, Chicago, IL 60601",
                DueDate                = DateTime.Today.AddDays(-1),
                TaskInstanceStatusName = "New",
                AssigneeEmployeeID     = null,
                Action                 = null
            },
            new()
            {
                TaskInstanceID         = 3,
                TaskTemplateName       = "Appraisal",
                PropertyAddress        = "789 Elm Rd, Naperville, IL 60540",
                DueDate                = DateTime.Today,
                TaskInstanceStatusName = "In Progress",
                AssigneeEmployeeID     = 42,
                Action                 = null
            },
            new()
            {
                TaskInstanceID         = 4,
                TaskTemplateName       = "Closing Docs",
                PropertyAddress        = "321 Pine St, Rockford, IL 61101",
                DueDate                = DateTime.Today.AddDays(14),
                TaskInstanceStatusName = "Completed",
                AssigneeEmployeeID     = 42,
                Action                 = null
            },
            new()
            {
                TaskInstanceID         = 5,
                TaskTemplateName       = "Insurance Verification",
                PropertyAddress        = "654 Cedar Blvd, Peoria, IL 61602",
                DueDate                = DateTime.Today.AddDays(7),
                TaskInstanceStatusName = "New",
                AssigneeEmployeeID     = null,
                Action                 = null
            }
        ];
    }
}
