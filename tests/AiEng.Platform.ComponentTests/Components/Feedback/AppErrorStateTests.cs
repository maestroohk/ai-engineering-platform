using AiEng.Platform.App.Components.Feedback;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Feedback;

public class AppErrorStateTests : BunitContext
{
    [Fact]
    public void Renders_Title()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "Cannot connect"));

        Assert.Contains("Cannot connect", cut.Find(".app-error-state-title").TextContent);
    }

    [Fact]
    public void Description_Renders_When_Supplied()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.Description, "Service unavailable."));

        Assert.Contains("Service unavailable.", cut.Find(".app-error-state-description").TextContent);
    }

    [Fact]
    public void ErrorCode_Renders_When_Supplied()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.ErrorCode, "E_PROVIDER_DOWN"));

        Assert.Contains("E_PROVIDER_DOWN", cut.Markup);
    }

    [Fact]
    public void CorrelationId_Renders_When_Supplied()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.CorrelationId, "abc-123"));

        Assert.Contains("abc-123", cut.Markup);
    }

    [Fact]
    public void Without_Diagnostics_Omits_Diagnostics_List()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t"));

        Assert.Empty(cut.FindAll(".app-error-state-diagnostics"));
    }

    [Fact]
    public void Primary_Action_Renders_When_Supplied()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.PrimaryAction, "<button class=\"probe\">Retry</button>"));

        Assert.NotNull(cut.Find(".app-error-state-actions .probe"));
    }

    [Fact]
    public void Secondary_Action_Renders_When_Supplied()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t")
            .Add(p => p.SecondaryAction, "<a class=\"probe\" href=\"/logs\">View logs</a>"));

        Assert.NotNull(cut.Find(".app-error-state-actions .probe"));
    }

    [Fact]
    public void Accessible_Error_Semantics_Are_Present()
    {
        var cut = Render<AppErrorState>(parameters => parameters
            .Add(p => p.Title, "t"));

        var root = cut.Find(".app-error-state");
        Assert.Equal("alert", root.GetAttribute("role"));
        Assert.Equal("assertive", root.GetAttribute("aria-live"));
    }
}
