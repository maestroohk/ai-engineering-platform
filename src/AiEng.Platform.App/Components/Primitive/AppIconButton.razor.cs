using AiEng.Platform.App.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AiEng.Platform.App.Components.Primitive;

public class AppIconButtonBase : ComponentBase
{
    [Parameter]
    public AppButtonVariant Variant { get; set; } = AppButtonVariant.Ghost;

    [Parameter]
    public AppButtonSize Size { get; set; } = AppButtonSize.Medium;

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public bool Loading { get; set; }

    [Parameter, EditorRequired]
    public string AriaLabel { get; set; } = string.Empty;

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string Type { get; set; } = "button";

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string EffectiveType => Type;

    protected string EffectiveTitle => Title ?? AriaLabel;

    protected bool DisabledOrLoading => Disabled || Loading;

    protected string CssClass
    {
        get
        {
            var baseClass = $"app-icon-button app-icon-button-{Variant.ToString().ToLowerInvariant()} app-icon-button-size-{Size.ToString().ToLowerInvariant()}";
            return string.IsNullOrWhiteSpace(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }

    protected async Task HandleClick(MouseEventArgs args)
    {
        if (DisabledOrLoading)
        {
            return;
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(args);
        }
    }
}
