namespace BlazorBusiness.Web.Components.Tasks.Grid
{
    public class TaskActionClickedEventArgs
    {
        public string Action { get; set; } = "View";
        public int TaskInstanceID { get; set; }
    }
}
