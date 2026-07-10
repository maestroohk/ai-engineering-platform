using AiEng.Platform.App.Components.Common;
using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Layout;

public class AppCardBase : ComponentBase
{
    [Parameter]
    public AppCardVariant Variant { get; set; } = AppCardVariant.Default;

    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string CssClass =>
        $"app-card app-card-{Variant.ToString().ToLowerInvariant()}";
}
