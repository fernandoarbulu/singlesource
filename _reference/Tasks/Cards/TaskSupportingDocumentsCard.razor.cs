using BlazorBusiness.Web.Models.Container;
using BlazorBusiness.Web.Services.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskSupportingDocumentsCard : ComponentBase
    {
        [Parameter]
        public TaskInstance? TaskInstance { get; set; }

        [Parameter]
        public EventCallback<List<FileUpload>> FilesChanged { get; set; }

        [Inject]
        protected ITaskInstanceApiService TaskInstanceApiService { get; set; }

        public ICollection<OrderLinkedFile> TaskInstanceFiles { get; set; } = new List<OrderLinkedFile>();
        private bool SelectButtonEnabled = true;
        private bool ShowFileDialog = false;
        private bool ShowConfirmRemove = false;
        private OrderLinkedFile? SelectedFile;
        private string ConfirmRemoveMessage = string.Empty;
        private ComponentState State = ComponentState.Loading;

        protected override async Task OnInitializedAsync()
        {
            await GetFiles();
        }

        private async Task GetFiles()
        {
            if (TaskInstance != null)
            {
                TaskInstanceFiles = await TaskInstanceApiService.GetFilesForTaskInstance(TaskInstance.TaskInstanceID);
            }
            RenderLoading(false);
        }

        private async Task OnSelect()
        {
            ShowFileDialog = true;
        }

        private async Task OnDialogClosed(bool visible)
        {
            ShowFileDialog = visible;

            if (!visible)
            {
                RenderLoading(true);
                await GetFiles();
            }
        }

        private void RenderLoading(bool isLoading)
        {
            State = ComponentState.Content;
            SelectButtonEnabled = true;

            if (isLoading)
            {
                State = ComponentState.Loading;
                SelectButtonEnabled = false; ;
            }
        }

        private async Task ConfirmRemove()
        {
            RenderLoading(true);
            ShowConfirmRemove = false;

            if (SelectedFile == null)
            {
                return;
            }

            var payload = new FileMetaDataTaskInstanceRequest
            {
                FileName = SelectedFile.FileName,
                TaskInstanceID = TaskInstance.TaskInstanceID,
                OrderDetailID = SelectedFile.OrderDetailId
            };

            await TaskInstanceApiService.DetachFileFromTaskInstance(payload);
            await GetFiles();
        }

        private void CancelRemove()
        {
            ShowConfirmRemove = false;
            SelectedFile = null;
        }

        private async Task OnRemoveFile(OrderLinkedFile file)
        {
            ConfirmRemoveMessage = $"Are you sure you want to remove {file.FileName} from the current task?";
            SelectedFile = file;
            ShowConfirmRemove = true;
        }
    }
}