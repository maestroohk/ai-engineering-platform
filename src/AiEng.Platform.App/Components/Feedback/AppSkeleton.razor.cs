using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Feedback;

public class AppSkeletonBase : ComponentBase
{
    [Parameter]
    public int Lines { get; set; } = 3;

    [Parameter]
    public string Height { get; set; } = "12px";

    [Parameter]
    public bool Rounded { get; set; }

    [Parameter]
    public bool FullLastLine { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string LineClass => Rounded ? "app-skeleton-line app-skeleton-line-rounded" : "app-skeleton-line";
}
