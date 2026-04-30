using BlazorBusiness.Web.Components.Tasks.Actions;
using BlazorBusiness.Web.Components.Tasks.Actions.Factories;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;
using Telerik.Blazor.Components;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskDetailActionCard : ComponentBase
    {
        [Parameter]
        public TaskInstance TaskInstanceForActionCard { get; set; } = new TaskInstance();

        public List<ActionComponentConfig> ActionComponents {  get; set; } = new List<ActionComponentConfig>();

        public bool ShowWizard { get; set; } = true;

        private void OnWizardFinish()
        {
            ShowWizard = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            ActionComponents.Clear();

            if (TaskInstanceForActionCard != null && TaskInstanceForActionCard.TaskInstanceID != 0 && ActionComponents.Count == 0) 
            {
                await BuildActionComponents(TaskInstanceForActionCard);
            }
        }

        /// <summary>
        /// This method will build the ActionComponents from the task instance being passed in.
        /// </summary>
        /// <param name="taskInstance"></param>
        /// <returns></returns>
        private async Task BuildActionComponents(TaskInstance taskInstance)
        {
            if (taskInstance.Actions != null && taskInstance.Actions.Count != 0)
            {
                foreach (var action in taskInstance.Actions)
                {
                    //Create an action based on the ActionTemplateName
                    var actionComponent = actionFactory.CreateAction(action.ActionTemplate.ActionTemplateName);
                    ActionComponents.Add(new ActionComponentConfig() { ActionComponentType = actionComponent.GetType() });
                }
            }
        }
    }

}
