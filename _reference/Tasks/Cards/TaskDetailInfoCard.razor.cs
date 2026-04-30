using BlazorBusiness.Web.Models.Container;
using BlazorBusiness.Web.Models.Tasking;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using System.Runtime.CompilerServices;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskDetailInfoCard : ComponentBase
    {
        [Parameter]
        public TaskDetailDialogModel DetailDialogModel { get; set; }
    }
}
