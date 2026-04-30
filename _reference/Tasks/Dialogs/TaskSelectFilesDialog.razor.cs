using BlazorBusiness.Web.Models.Container;
using BlazorBusiness.Web.Models.Orders;
using BlazorBusiness.Web.Services.Orders;
using BlazorBusiness.Web.Services.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using System.Runtime.CompilerServices;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Dialogs
{
    public partial class TaskSelectFilesDialog
    {
        [Parameter] 
        public bool Visible { get; set; }

        [Parameter] 
        public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter]
        public required TaskInstance TaskInstance { get; set; }

        [Inject]
        protected IOrdersService OrdersService { get; set; }

        [Inject]
        protected ITaskInstanceApiService TaskInstanceApiService { get; set; }

        private string SelectedSort = "Name";
        private List<string> SortOptions = new() { "Name", "Date", "Size" };
        private bool SortAscending = true;
        private string SearchText = string.Empty;

        private List<LinkWorkOrder> Folders = new();
        private LinkWorkOrder? SelectedFolder;
        private List<OrderLinkedFile> SelectedFolderFiles = new();
        private List<OrderLinkedFile> AllFolderFiles = new();

        private TelerikFileSelect? FileSelectRef;
        private OrderLinkedFile? SelectedFile;
        private List<FileUpload> PendingUploads = new();

        public required TelerikDialog DialogRef;
        private ComponentState State = ComponentState.Loading;

        private bool ShowConfirmUpload = false;
        private string ConfirmUploadMessage = string.Empty;
        private bool ShowConfirmAssociation = false;
        private string ConfirmAssociationMessage = string.Empty;
        private OrderLinkedFile? PendingAssociationFile;

        private void ToggleSortDirection()
        {
            SortAscending = !SortAscending;
            ApplyFilters();
        }

        private void OnSortChanged(string newValue)
        {
            SelectedSort = newValue;
            ApplyFilters();
        }

        private bool IsAssociated(OrderLinkedFile file)
        {
            return file.TaskInstanceIDs != null
                && TaskInstance != null
                && file.TaskInstanceIDs.Contains(TaskInstance.TaskInstanceID);
        }

        private void ApplyFilters()
        {
            RenderLoading(true);
            IEnumerable<OrderLinkedFile> query = AllFolderFiles;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(file =>
                    !string.IsNullOrWhiteSpace(file.FileName) &&
                    file.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            query = SelectedSort switch
            {
                "Name" =>
                    SortAscending
                        ? query.OrderBy(file => file.FileName)
                        : query.OrderByDescending(file => file.FileName),

                "Date" =>
                    SortAscending
                        ? query.OrderBy(file => file.FileOriginallyUploaded)
                        : query.OrderByDescending(file => file.FileOriginallyUploaded),

                "Size" =>
                    SortAscending
                        ? query.OrderBy(file => file.FileSize)
                        : query.OrderByDescending(file => file.FileSize),

                _ => query
            };

            SelectedFolderFiles = query.ToList();
            RenderLoading(false);
        }
        private void OnSearchChanged(string value)
        {
            SearchText = value;
            ApplyFilters();
        }

        private void UploadFiles()
        {
            if (FileSelectRef != null)
            {
                FileSelectRef.OpenSelectFilesDialog();
            }
        }
        protected async override Task OnParametersSetAsync()
        {
            if (Visible)
            {
                await GetFolders();
            }
        }

        private void RenderLoading(bool loading)
        {
            // reusable method for toggling loading gif
            if (DialogRef != null)
            {
                State = loading ? ComponentState.Loading : ComponentState.Content;
                DialogRef.Refresh();
            }
        }

        private async Task GetFolders()
        {
            RenderLoading(true);

            var orderDetailID = TaskInstance.OrderDetailID;
            if (orderDetailID == null)
            {
                RenderLoading(false);
                return;
            }

            Folders = await OrdersService.GetEligibleLinkOrders(orderDetailID.Value);
            SelectTaskFolder();

            if (SelectedFolder != null)
            {
                await SelectFolder(SelectedFolder);
            }
        }

        private void SelectTaskFolder()
        {
            SelectedFolder = Folders.FirstOrDefault(f => f.OrderDetailID == TaskInstance.OrderDetailID);
        }

        private async Task GetFolderFiles(int? orderDetailID)
        {
            if (orderDetailID == null)
            {
                AllFolderFiles = new();
                SelectedFolderFiles = new();
                return;
            }

            AllFolderFiles = await OrdersService.GetOrderDetailFiles(orderDetailID.Value);
            SelectedFile = null;

            ApplyFilters();
        }

        private bool DialogVisible
        {
            get => Visible;
            set
            {
                if (Visible != value)
                {
                    Visible = value;
                    _ = VisibleChanged.InvokeAsync(value);
                }
            }
        }

        private async Task SelectFolder(LinkWorkOrder folder)
        {
            SelectedFolder = folder;
            RenderLoading(true);
            await GetFolderFiles(folder.OrderDetailID);
        }

        private async Task OnUpload(FileSelectEventArgs args)
        {
            PendingUploads = new List<FileUpload>();

            foreach (var file in args.Files)
            {
                using var ms = new MemoryStream();
                await file.Stream.CopyToAsync(ms);

                PendingUploads.Add(new FileUpload
                {
                    FileName = file.Name,
                    File = ms.ToArray(),
                    VendorVisible = true,
                    ClientVisible = true,
                    BusinessGroupLimits = new List<int> { 0 }
                });
            }

            if (PendingUploads.Count > 0)
            {
                ConfirmUploadMessage =
                    $"Are you sure you want to upload the following files: " +
                    string.Join(", ", PendingUploads.Select(file => file.FileName));

                ShowConfirmUpload = true;
            }
        }

        private async Task ConfirmUpload()
        {
            ShowConfirmUpload = false;
            RenderLoading(true);

            if (TaskInstance == null || TaskInstance.OrderDetailID == null)
            {
                return;
            }
            int orderDetailID = TaskInstance.OrderDetailID.Value;

            await OrdersService.PostOrderDetailFile(orderDetailID, PendingUploads);

            SelectTaskFolder();
            if (SelectedFolder == null)
            {
                return;
            }
            await GetFolderFiles(SelectedFolder.OrderDetailID);

            PendingUploads.Clear();
        }

        private void CancelUpload()
        {
            ShowConfirmUpload = false;
            PendingUploads.Clear();
        }

        private void OnAssociate(OrderLinkedFile file)
        {
            bool currentlyAssociated = IsAssociated(file);
            PendingAssociationFile = file;

            ConfirmAssociationMessage = $"Attach '{file.FileName}' to task {TaskInstance.TaskInstanceID}?";
            ShowConfirmAssociation = true;
        }

        private async Task ConfirmAssociation()
        {
            RenderLoading(true);
            ShowConfirmAssociation = false;

            if (SelectedFolder == null || PendingAssociationFile == null)
                return;

            var file = PendingAssociationFile;

            var payload = new FileMetaDataTaskInstanceRequest
            {
                FileName = file.FileName,
                TaskInstanceID = TaskInstance.TaskInstanceID,
                OrderDetailID = file.OrderDetailId
            };

            SelectedFile = null;
            PendingAssociationFile = null;

            await TaskInstanceApiService.AttachFileToTaskInstance(payload);
            await GetFolderFiles(SelectedFolder.OrderDetailID);
        }

        private void CancelAssociation()
        {
            ShowConfirmAssociation = false;
            PendingAssociationFile = null;
        }


        private void SelectFile(OrderLinkedFile file)
        {
            SelectedFile = file;
            DialogRef.Refresh();
        }
    }
}
