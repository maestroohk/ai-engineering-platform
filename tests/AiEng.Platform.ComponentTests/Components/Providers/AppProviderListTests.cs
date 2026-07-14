using AiEng.Platform.App.Components.Providers;
using AiEng.Platform.Application.Providers;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Providers;

public class AppProviderListTests : BunitContext
{
    [Fact]
    public void Renders_Populated_Slot_With_One_Card_Per_Provider()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, "2.45.0", new Dictionary<string, string>()),
            new("ollama", "Ollama", ProviderFamily.AgentRuntime, ProviderStatus.Available, "0.3.12", new Dictionary<string, string>()),
            new("docker", "Docker", ProviderFamily.QualityGate, ProviderStatus.Unavailable, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        var items = cut.FindAll(".app-provider-list-item");
        Assert.Equal(3, items.Count);
        Assert.Contains("Git CLI", cut.Markup);
        Assert.Contains("Ollama", cut.Markup);
        Assert.Contains("Docker", cut.Markup);
    }

    [Fact]
    public void Renders_Empty_Slot_When_No_Providers()
    {
        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, Array.Empty<ProviderDescriptor>()));

        Assert.NotNull(cut.Find(".app-empty-state"));
        Assert.Contains("No providers registered", cut.Markup);
    }

    [Fact]
    public void Renders_Loading_Slot_When_IsLoading_Is_True()
    {
        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.IsLoading, true)
            .Add(p => p.Providers, Array.Empty<ProviderDescriptor>()));

        Assert.NotNull(cut.Find(".app-loading"));
        Assert.Contains("Loading providers", cut.Markup);
    }

    [Fact]
    public void Renders_Error_Slot_When_ErrorMessage_Is_Set()
    {
        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.ErrorMessage, "Failed to look up providers")
            .Add(p => p.ErrorCode, "PROVIDER_LOOKUP_FAILED")
            .Add(p => p.CorrelationId, "corr-1")
            .Add(p => p.Providers, Array.Empty<ProviderDescriptor>()));

        Assert.NotNull(cut.Find(".app-error-state"));
        Assert.Contains("Cannot load providers", cut.Markup);
        Assert.Contains("Failed to look up providers", cut.Markup);
        Assert.Contains("PROVIDER_LOOKUP_FAILED", cut.Markup);
    }

    [Fact]
    public void Renders_DisplayName_Per_Provider()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>()),
            new("ollama", "Ollama", ProviderFamily.AgentRuntime, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        var names = cut.FindAll(".app-provider-list-name");
        Assert.Equal(2, names.Count);
        Assert.Contains("Git CLI", cut.Markup);
        Assert.Contains("Ollama", cut.Markup);
    }

    [Fact]
    public void Renders_Status_Dot_Success_For_Available_Status()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("app-status-dot-success", cut.Markup);
        Assert.DoesNotContain("app-status-dot-error", cut.Markup);
    }

    [Fact]
    public void Renders_Status_Dot_Error_For_Unavailable_Status()
    {
        var providers = new ProviderDescriptor[]
        {
            new("docker", "Docker", ProviderFamily.QualityGate, ProviderStatus.Unavailable, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("app-status-dot-error", cut.Markup);
        Assert.DoesNotContain("app-status-dot-success", cut.Markup);
    }

    [Fact]
    public void Renders_Disabled_Badge_For_Disabled_Status()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Disabled, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("Disabled", cut.Markup);
        Assert.Contains("app-status-dot-neutral", cut.Markup);
    }

    [Fact]
    public void Renders_Version_Per_Provider_When_Not_Null()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, "2.45.0", new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("2.45.0", cut.Markup);
        var versions = cut.FindAll(".app-provider-list-version");
        Assert.NotEmpty(versions);
    }

    [Fact]
    public void Renders_Muted_Version_Placeholder_When_Version_Is_Null()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("version unknown", cut.Markup);
        Assert.Contains("app-provider-list-version-muted", cut.Markup);
    }

    [Fact]
    public void Renders_Metadata_As_KeyValueList_When_Non_Empty()
    {
        var metadata = new Dictionary<string, string>
        {
            ["Path"] = "/usr/bin/git",
            ["Command"] = "git"
        };
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, "2.45.0", metadata)
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.Contains("Path", cut.Markup);
        Assert.Contains("/usr/bin/git", cut.Markup);
        Assert.Contains("Command", cut.Markup);
        Assert.Contains("git", cut.Markup);
    }

    [Fact]
    public void Omits_Metadata_Section_When_Metadata_Is_Empty()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        Assert.DoesNotContain("app-provider-list-metadata", cut.Markup);
    }

    [Fact]
    public void Custom_Populated_Slot_Overrides_Default_Rendering()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        RenderFragment customPopulated = builder =>
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "custom-populated-slot");
            builder.AddContent(2, "CUSTOM-POPULATED-CONTENT");
            builder.CloseElement();
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers)
            .Add(p => p.Populated, customPopulated));

        Assert.NotNull(cut.Find(".custom-populated-slot"));
        Assert.Contains("CUSTOM-POPULATED-CONTENT", cut.Markup);
        Assert.DoesNotContain("app-provider-list-item", cut.Markup);
    }

    [Fact]
    public void Populated_List_Has_AriaLive_Polite()
    {
        var providers = new ProviderDescriptor[]
        {
            new("git-cli", "Git CLI", ProviderFamily.Git, ProviderStatus.Available, null, new Dictionary<string, string>())
        };

        var cut = Render<AppProviderList>(parameters => parameters
            .Add(p => p.Providers, providers));

        var list = cut.Find("[role=\"list\"]");
        Assert.Equal("polite", list.GetAttribute("aria-live"));
    }
}
