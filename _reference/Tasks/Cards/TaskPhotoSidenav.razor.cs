using BlazorBusiness.Web.Components.Tasks.Actions;
using BlazorBusiness.Web.Components.Tasks.Actions.Factories;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskPhotoSidenav
    {
        [Parameter] public TaskInstance? TaskInstance { get; set; }

        [Parameter] public EventCallback OnClose { get; set; }
    }
}
