using AiEng.Platform.App.Components.Common;
using Microsoft.AspNetCore.Components;

namespace AiEng.Platform.App.Components.Primitive;

public class AppStackBase : ComponentBase
{
    [Parameter]
    public AppStackDirection Direction { get; set; } = AppStackDirection.Vertical;

    [Parameter]
    public AppStackAlign Align { get; set; } = AppStackAlign.Stretch;

    [Parameter]
    public AppStackJustify Justify { get; set; } = AppStackJustify.Start;

    [Parameter]
    public AppStackWrap Wrap { get; set; } = AppStackWrap.NoWrap;

    [Parameter]
    public AppStackGap Gap { get; set; } = AppStackGap.Medium;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string CssClass
    {
        get
        {
            var direction = Direction == AppStackDirection.Vertical ? "flex-col" : "flex-row";
            var align = Align.ToString().ToLowerInvariant() switch
            {
                "start" => "items-start",
                "center" => "items-center",
                "end" => "items-end",
                "baseline" => "items-baseline",
                _ => "items-stretch"
            };
            var justify = Justify.ToString().ToLowerInvariant() switch
            {
                "start" => "justify-start",
                "center" => "justify-center",
                "end" => "justify-end",
                "between" => "justify-between",
                "around" => "justify-around",
                "evenly" => "justify-evenly",
                _ => "justify-start"
            };
            var wrap = Wrap switch
            {
                AppStackWrap.Wrap => "flex-wrap",
                AppStackWrap.WrapReverse => "flex-wrap-reverse",
                _ => "flex-nowrap"
            };
            var gap = Gap switch
            {
                AppStackGap.None => "gap-0",
                AppStackGap.ExtraSmall => "gap-1",
                AppStackGap.Small => "gap-2",
                AppStackGap.Medium => "gap-4",
                AppStackGap.Large => "gap-6",
                AppStackGap.ExtraLarge => "gap-8",
                _ => "gap-4"
            };
            return $"app-stack flex {direction} {align} {justify} {wrap} {gap}";
        }
    }
}
