using BlazorBusiness.Web.Components.Tasks.Actions.Interfaces;
using System.Reflection;

namespace BlazorBusiness.Web.Components.Tasks.Actions.Factories
{
    public class ActionFactory : IActionFactory
    {
        public const string ActionNamespace = "BlazorBusiness.Web.Components.Tasks.Actions";

        public IActionComponent CreateAction(string actionTemplateName)
        {
            var action = Type.GetType(ActionNamespace + "." + actionTemplateName.Replace(" ", ""));
            if (action != null)
            {
                return (IActionComponent)Activator.CreateInstance(action);
            }
            throw new ArgumentException("Invalid Action class or namespace");
        }
    }
}
