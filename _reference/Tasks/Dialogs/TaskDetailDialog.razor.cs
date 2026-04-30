using BlazorBusiness.Web.Models.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Dialogs
{
    public partial class TaskDetailDialog
    {
        [Parameter]
        public bool Visible { get; set; }
        
        [Parameter]
        public string Title { get; set; }
        
        [Parameter]
        public EventCallback VisibleChanged { get; set; }        

        [Parameter]
        public int TaskInstanceID { get; set; }
        public TaskInstance TaskInstance { get; set; } = new TaskInstance();
        public TaskDetailDialogModel TaskDetailDialogModel { get; set; } = new TaskDetailDialogModel();
        public List<KeyValuePair<string, string>> WorkOrderDetailLayoutFields { get; set; } = new List<KeyValuePair<string, string>>();

        public string ReferenceNumberForCard { get; set; }

        public TelerikDialog DialogRef;

        async Task OnVisibleChanged(bool currVisible)
        {
            this.Visible = currVisible;
            await this.VisibleChanged.InvokeAsync();

        }
        protected override async Task OnParametersSetAsync()
        {
            if (TaskInstanceID > 0 && Visible)
            {
                if (TaskInstance == null || TaskInstance.TaskInstanceID != TaskInstanceID)
                {
                    TaskInstance = await taskInstanceViewService.GetTaskInstance(TaskInstanceID, true);

                    TaskDetailDialogModel = await taskInstanceViewService.MapTaskInstancesIntoTaskDetailDialogModel(TaskInstance);


                }

                if (TaskInstance.OrderDetailID is not null && TaskInstance.OrderDetailID.Value > 0 && WorkOrderDetailLayoutFields.Count == 0)
                {
                    WorkOrderDetailLayoutFields.Add(new KeyValuePair<string, string>("Work Order", ""));
                    WorkOrderDetailLayoutFields.Add(new KeyValuePair<string, string>("Due Date", ""));
                    WorkOrderDetailLayoutFields.Add(new KeyValuePair<string, string>("Property Address", ""));
                    WorkOrderDetailLayoutFields.Add(new KeyValuePair<string, string>("Client", ""));
                    WorkOrderDetailLayoutFields = await GetWorkOrderDetailByOrderDetailID(TaskInstance.OrderDetailID.Value);
                }

                ReferenceNumberForCard = WorkOrderDetailLayoutFields.Where(x => x.Key.Equals("Work Order")).First().Value;
                TaskDetailDialogModel.ReferenceNumber = ReferenceNumberForCard;
                DialogRef.Refresh();
            }
        }

        private async Task<List<KeyValuePair<string, string>>> GetWorkOrderDetailByOrderDetailID(int orderDetailID)
        {
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

            TaskWorkOrderDetail taskWorkOrderDetail = await taskInstanceViewService.GetWorkOrderDetailByOrderDetailID(orderDetailID);
            if (taskWorkOrderDetail != null)
            {
                foreach (var WorkOrderDetailLayoutField in WorkOrderDetailLayoutFields)
                {
                    switch (WorkOrderDetailLayoutField.Key.ToString().ToLower().Trim())
                    {
                        case "work order":
                            keyValuePairs.Add(new KeyValuePair<string, string>(WorkOrderDetailLayoutField.Key, taskWorkOrderDetail.OrderNumber));
                            break;
                        case "due date":
                            keyValuePairs.Add(new KeyValuePair<string, string>(WorkOrderDetailLayoutField.Key, taskWorkOrderDetail.DueDate.ToString()));
                            break;
                        case "client":
                            keyValuePairs.Add(new KeyValuePair<string, string>(WorkOrderDetailLayoutField.Key, taskWorkOrderDetail.CustomerName.ToString()));
                            break;
                        case "property address":
                            keyValuePairs.Add(new KeyValuePair<string, string>(WorkOrderDetailLayoutField.Key, taskWorkOrderDetail.PropertyAddress.ToString()));
                            break;
                    }
                }
            }
            return keyValuePairs;
        }
    }
}
