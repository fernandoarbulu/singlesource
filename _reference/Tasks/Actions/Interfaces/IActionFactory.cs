namespace BlazorBusiness.Web.Components.Tasks.Actions.Interfaces
{
    public interface IActionFactory
    {
        public IActionComponent CreateAction(string actionTemplateName);
    }
}
