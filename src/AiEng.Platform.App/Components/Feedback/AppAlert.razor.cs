using AiEng.Platform.App.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AiEng.Platform.App.Components.Feedback;

public class AppAlertBase : ComponentBase
{
    [Parameter]
    public AppAlertVariant Variant { get; set; } = AppAlertVariant.Information;

    [Parameter]
    public RenderFragment? Title { get; set; }

    [Parameter]
    public RenderFragment? Description { get; set; }

    [Parameter]
    public RenderFragment? Actions { get; set; }

    [Parameter]
    public bool Dismissible { get; set; }

    [Parameter]
    public EventCallback OnDismiss { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string CssClass =>
        $"app-alert app-alert-{Variant.ToString().ToLowerInvariant()}";

    protected async Task HandleDismiss(MouseEventArgs args)
    {
        if (OnDismiss.HasDelegate)
        {
            await OnDismiss.InvokeAsync();
        }
    }
}
