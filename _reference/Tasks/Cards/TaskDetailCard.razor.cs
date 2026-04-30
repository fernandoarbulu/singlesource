using BlazorBusiness.Web.Brokers.Api;
using Microsoft.AspNetCore.Components;
using SingleSource.WebAPI.Business.SDK;

namespace BlazorBusiness.Web.Components.Tasks.Cards
{
    public partial class TaskDetailCard : ComponentBase
    {
        [Inject]
        IBusinessApiBroker businessApiBroker { get; set; }

        [Parameter]
        public TaskInstance TaskInstance { get; set; }

        private BaseOrderForDetailReview? OrderWithExpenses { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (TaskInstance?.OrderDetailID != null && TaskInstance.OrderDetailID > 0)
            {
                //Using the endpoint /api/business/bod-orders/lite
                BaseOrderForDetailReviewsResponse baseOrders = await businessApiBroker.GetLiteBaseOrdersByOrderDetailIds([TaskInstance.OrderDetailID.Value]);

                BaseOrderForDetailReviewsResponse response = await businessApiBroker.ExtendWithExpenses(new List<BaseOrderForDetailReview> { baseOrders.BaseOrderForDetailReviews.First() });
                if (response?.BaseOrderForDetailReviews != null && response.BaseOrderForDetailReviews.Any())
                {
                    OrderWithExpenses = response.BaseOrderForDetailReviews.First();
                }
            }
        }

        private string GetTimeElapsed()
        {
            if (TaskInstance?.AssignedDate == null)
                return "N/A";

            TimeSpan elapsed = DateTimeOffset.Now - TaskInstance.AssignedDate.Value;
            return $"{(int)elapsed.TotalDays}D {elapsed.Hours}H";
        }
    }
}
