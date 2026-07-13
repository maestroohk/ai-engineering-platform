using AiEng.Platform.App.Components.Common;
using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Diagnostics;

public class AppKeyValueListBase : ComponentBase
{
    [Parameter, EditorRequired]
    public IReadOnlyList<KeyValuePair<string, string>> Items { get; set; } = default!;

    [Parameter]
    public AppKeyValueListFormat Format { get; set; } = AppKeyValueListFormat.Plain;

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public string? ErrorMessage { get; set; }

    [Parameter]
    public string? ErrorCode { get; set; }

    [Parameter]
    public string? CorrelationId { get; set; }

    [Parameter]
    public RenderFragment? Loading { get; set; }

    [Parameter]
    public RenderFragment? Empty { get; set; }

    [Parameter]
    public RenderFragment? Error { get; set; }

    [Parameter]
    public RenderFragment? Populated { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
