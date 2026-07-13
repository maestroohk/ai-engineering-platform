using AiEng.Platform.App.Components.Diagnostics;
using AiEng.Platform.Application.Capabilities;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Diagnostics;

public class AppCapabilityListTests : BunitContext
{
    [Fact]
    public void Renders_Populated_Slot_With_One_Card_Per_Capability()
    {
        var capabilities = new HostCapability[]
        {
            new("git", true, "2.45.0", false, null),
            new("dotnet", true, "10.0.0", false, null),
            new("docker", false, null, false, null)
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        var items = cut.FindAll(".app-capability-list-item");
        Assert.Equal(3, items.Count);
        Assert.Contains("git", cut.Markup);
        Assert.Contains("dotnet", cut.Markup);
        Assert.Contains("docker", cut.Markup);
    }

    [Fact]
    public void Renders_Empty_Slot_When_No_Capabilities()
    {
        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, Array.Empty<HostCapability>()));

        Assert.NotNull(cut.Find(".app-empty-state"));
        Assert.Contains("No capabilities detected", cut.Markup);
    }

    [Fact]
    public void Renders_Loading_Slot_When_IsLoading_Is_True()
    {
        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Capabilities, Array.Empty<HostCapability>()));

        Assert.NotNull(cut.Find(".app-loading"));
        Assert.Contains("Detecting host capabilities", cut.Markup);
    }

    [Fact]
    public void Renders_Error_Slot_When_ErrorMessage_Is_Set()
    {
        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.ErrorMessage, "Failed to detect capabilities")
            .Add(p => p.ErrorCode, "HOST_PROBE_FAILED")
            .Add(p => p.CorrelationId, "corr-1")
            .Add(p => p.Capabilities, Array.Empty<HostCapability>()));

        Assert.NotNull(cut.Find(".app-error-state"));
        Assert.Contains("Cannot load capabilities", cut.Markup);
        Assert.Contains("Failed to detect capabilities", cut.Markup);
        Assert.Contains("HOST_PROBE_FAILED", cut.Markup);
    }

    [Fact]
    public void Renders_Status_Dot_Success_For_Available_True()
    {
        var capabilities = new HostCapability[]
        {
            new("git", true, "2.45.0", false, null)
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        Assert.Contains("app-status-dot-success", cut.Markup);
        Assert.DoesNotContain("app-status-dot-error", cut.Markup);
    }

    [Fact]
    public void Renders_Status_Dot_Error_For_Available_False()
    {
        var capabilities = new HostCapability[]
        {
            new("docker", false, null, false, null)
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        Assert.Contains("app-status-dot-error", cut.Markup);
        Assert.DoesNotContain("app-status-dot-success", cut.Markup);
    }

    [Fact]
    public void Renders_Credential_Set_Badge_When_CredentialAvailable_Is_True()
    {
        var capabilities = new HostCapability[]
        {
            new("docker", true, "24.0.0", true, "docker-hub")
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        Assert.Contains("Credential set", cut.Markup);
        Assert.Contains("app-badge-success", cut.Markup);
    }

    [Fact]
    public void Omits_Credential_Set_Badge_When_CredentialAvailable_Is_False()
    {
        var capabilities = new HostCapability[]
        {
            new("git", true, "2.45.0", false, null)
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        Assert.DoesNotContain("Credential set", cut.Markup);
    }

    [Fact]
    public void Custom_Populated_Slot_Overrides_Default_Rendering()
    {
        var capabilities = new HostCapability[]
        {
            new("git", true, "2.45.0", false, null)
        };

        RenderFragment customPopulated = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-populated-slot");
            builder.AddContent(2, "CUSTOM-POPULATED-CONTENT");
            builder.CloseElement();
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities)
            .Add(p => p.Populated, customPopulated));

        Assert.NotNull(cut.Find(".custom-populated-slot"));
        Assert.Contains("CUSTOM-POPULATED-CONTENT", cut.Markup);
        Assert.DoesNotContain("app-capability-list-item", cut.Markup);
    }

    [Fact]
    public void Custom_Loading_Slot_Overrides_Default_Loading()
    {
        RenderFragment customLoading = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-loading-slot");
            builder.AddContent(2, "CUSTOM-LOADING");
            builder.CloseElement();
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Capabilities, Array.Empty<HostCapability>())
            .Add(p => p.Loading, customLoading));

        Assert.NotNull(cut.Find(".custom-loading-slot"));
        Assert.DoesNotContain("Detecting host capabilities", cut.Markup);
    }

    [Fact]
    public void Custom_Empty_Slot_Overrides_Default_Empty()
    {
        RenderFragment customEmpty = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-empty-slot");
            builder.AddContent(2, "CUSTOM-EMPTY");
            builder.CloseElement();
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, Array.Empty<HostCapability>())
            .Add(p => p.Empty, customEmpty));

        Assert.NotNull(cut.Find(".custom-empty-slot"));
        Assert.DoesNotContain("No capabilities detected", cut.Markup);
    }

    [Fact]
    public void Custom_Error_Slot_Overrides_Default_Error()
    {
        RenderFragment customError = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-error-slot");
            builder.AddContent(2, "CUSTOM-ERROR");
            builder.CloseElement();
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.ErrorMessage, "boom")
            .Add(p => p.Capabilities, Array.Empty<HostCapability>())
            .Add(p => p.Error, customError));

        Assert.NotNull(cut.Find(".custom-error-slot"));
        Assert.DoesNotContain("Cannot load capabilities", cut.Markup);
    }

    [Fact]
    public void Populated_List_Has_AriaLive_Polite()
    {
        var capabilities = new HostCapability[]
        {
            new("git", true, "2.45.0", false, null)
        };

        var cut = Render<AppCapabilityList>(parameters => parameters
            .Add(p => p.Capabilities, capabilities));

        var list = cut.Find("[role=\"list\"]");
        Assert.Equal("polite", list.GetAttribute("aria-live"));
    }
}
