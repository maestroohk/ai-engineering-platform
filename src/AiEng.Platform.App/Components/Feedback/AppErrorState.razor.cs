using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Feedback;

public class AppErrorStateBase : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment Title { get; set; } = default!;

    [Parameter]
    public RenderFragment? Description { get; set; }

    [Parameter]
    public string? ErrorCode { get; set; }

    [Parameter]
    public string? CorrelationId { get; set; }

    [Parameter]
    public RenderFragment? PrimaryAction { get; set; }

    [Parameter]
    public RenderFragment? SecondaryAction { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected bool HasDiagnostics =>
        !string.IsNullOrWhiteSpace(ErrorCode) || !string.IsNullOrWhiteSpace(CorrelationId);

    protected string CssClass => "app-error-state";
}
