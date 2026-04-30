using Microsoft.AspNetCore.Components;

namespace BlazorBusiness.Web.Components.Tasks.Chips
{
    public partial class TextChip
    {
        /// <summary>
        /// The key of this parameter should be the text you wish to display in the chip, the value should be the telerik themeColor string identifier. For a list of these indentifiers, see the documentation here: https://docs.telerik.com/blazor-ui/components/chip/appearance#themecolor
        /// </summary>
        [Parameter]
        public KeyValuePair<string, string> TextWithThemeColor { get; set; }
        [Parameter]
        public EventCallback OnChipClicked { get; set; }
        [Parameter]
        public string Class { get; set; }
        public string ThemeColor { get; set; }
        public string ChipText { get; set; }
        protected override void OnInitialized()
        {
            ThemeColor = TextWithThemeColor.Value;
            ChipText = TextWithThemeColor.Key;
        }

        public async Task ChipClickEventHandler()
        {
            await OnChipClicked.InvokeAsync();
        }

    }
}
