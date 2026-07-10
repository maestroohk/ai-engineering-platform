using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Layout;

public class AppSectionBase : ComponentBase
{
    [Parameter]
    public RenderFragment? Title { get; set; }

    [Parameter]
    public RenderFragment? Subtitle { get; set; }

    [Parameter]
    public RenderFragment? Actions { get; set; }

    [Parameter]
    public RenderFragment? Content { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected RenderFragment EffectiveContent => Content ?? ChildContent ?? (builder => { });
}
