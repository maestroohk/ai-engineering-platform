using System.ComponentModel;
using System.Runtime.CompilerServices;
using AiEng.Platform.App.Components.Projects;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Domain.Projects;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiEng.Platform.ComponentTests.Projects;

public class AppProjectCardTests : BunitContext
{
    public AppProjectCardTests()
    {
        Services.AddSingleton<IPlatformInfo>(new FakePlatformInfo(isWindows: true));
        Services.AddSingleton<IProcessRunner>(new FakeProcessRunner());
    }

    [Fact]
    public void Renders_Project_Name_And_Path()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("alpha", cut.Find(".app-project-card-title").TextContent);
        Assert.Contains("/tmp/alpha", cut.Find(".app-project-card-path").TextContent);
    }

    [Fact]
    public void Renders_New_Badge_When_LastUsedAt_Is_Null()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("New", cut.Markup);
        Assert.Contains("app-badge-neutral", cut.Markup);
    }

    [Fact]
    public void Renders_Active_Badge_When_LastUsedAt_Is_Present()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        project.Touch(new DateTimeOffset(2026, 7, 11, 9, 0, 0, TimeSpan.Zero));

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Active", cut.Markup);
        Assert.Contains("app-badge-success", cut.Markup);
    }

    [Fact]
    public void Renders_Open_Rename_And_Unregister_Buttons()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        Assert.Contains("Open", cut.Markup);
        Assert.Contains("Rename", cut.Markup);
        Assert.Contains("Unregister", cut.Markup);
        Assert.Contains("data-testid=\"open-project\"", cut.Markup);
        Assert.Contains("data-testid=\"rename-project\"", cut.Markup);
        Assert.Contains("data-testid=\"unregister-project\"", cut.Markup);
    }

    [Fact]
    public void Rename_Button_Is_Enabled_In_M3_2()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var rename = cut.Find("[data-testid='rename-project']");
        Assert.False(rename.HasAttribute("disabled"));
    }

    [Fact]
    public void Unregister_Button_Is_Enabled_In_M3_2()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var unregister = cut.Find("[data-testid='unregister-project']");
        Assert.False(unregister.HasAttribute("disabled"));
    }

    [Fact]
    public async Task Clicking_Rename_Invokes_OnRename()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var renameCount = 0;
        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project)
            .Add(p => p.OnRename, () => { renameCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='rename-project']").Click();

        Assert.Equal(1, renameCount);
    }

    [Fact]
    public async Task Clicking_Unregister_Invokes_OnUnregister()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var unregisterCount = 0;
        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project)
            .Add(p => p.OnUnregister, () => { unregisterCount++; return Task.CompletedTask; }));

        cut.Find("[data-testid='unregister-project']").Click();

        Assert.Equal(1, unregisterCount);
    }

    [Fact]
    public void Open_Button_Is_Enabled_When_Host_Is_Windows()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        RegisterServices(isWindows: true, runner: new FakeProcessRunner());

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var open = cut.Find("[data-testid='open-project']");
        Assert.False(open.HasAttribute("disabled"));
    }

    [Fact]
    public void Open_Button_Is_Disabled_When_Host_Is_Not_Windows()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        RegisterServices(isWindows: false, runner: new FakeProcessRunner());

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var open = cut.Find("[data-testid='open-project']");
        Assert.True(open.HasAttribute("disabled"));
    }

    [Fact]
    public void Clicking_Open_Invokes_IProcessRunner_With_Explorer_And_ProjectPath()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var runner = new FakeProcessRunner();
        RegisterServices(isWindows: true, runner: runner);

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        cut.Find("[data-testid='open-project']").Click();

        cut.WaitForState(() => runner.RunToCompletionCallCount == 1);

        Assert.Equal("explorer.exe", runner.LastExecutable);
        Assert.NotNull(runner.LastArguments);
        Assert.Single(runner.LastArguments!);
    }

    [Fact]
    public void Open_Click_Passes_ProjectPath_Single_Element_As_Argument()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var runner = new FakeProcessRunner();
        RegisterServices(isWindows: true, runner: runner);

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        cut.Find("[data-testid='open-project']").Click();

        cut.WaitForState(() => runner.RunToCompletionCallCount == 1);

        Assert.Equal("/tmp/alpha", runner.LastArguments![0]);
    }

    [Fact]
    public void Open_Click_Swallows_Process_Exceptions()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        var runner = new FakeProcessRunner
        {
            ThrowOnRunToCompletionAsync = new Win32Exception("explorer.exe not found"),
        };
        RegisterServices(isWindows: true, runner: runner);

        var cut = Render<AppProjectCard>(parameters => parameters
            .Add(p => p.Project, project));

        var ex = Record.Exception(() => cut.Find("[data-testid='open-project']").Click());

        Assert.Null(ex);
        cut.WaitForState(() => cut.Markup.Contains("app-project-card-open-error"));
        Assert.Contains("Could not open the project folder", cut.Markup);
    }

    private void RegisterServices(bool isWindows, IProcessRunner runner)
    {
        Services.AddSingleton<IPlatformInfo>(new FakePlatformInfo(isWindows));
        Services.AddSingleton<IProcessRunner>(runner);
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero));

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public string? LastExecutable { get; private set; }

        public IReadOnlyList<string>? LastArguments { get; private set; }

        public int RunToCompletionCallCount { get; private set; }

        public Exception? ThrowOnRunToCompletionAsync { get; set; }

        public async IAsyncEnumerable<string> RunAsync(
            string executable,
            IReadOnlyList<string> arguments,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            yield break;
        }

        public Task<ProcessResult> RunToCompletionAsync(
            string executable,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            RunToCompletionCallCount++;
            LastExecutable = executable;
            LastArguments = arguments;
            if (ThrowOnRunToCompletionAsync is { } ex)
            {
                throw ex;
            }
            return Task.FromResult(ProcessResult.From(0, string.Empty, string.Empty));
        }
    }

    private sealed class FakePlatformInfo : IPlatformInfo
    {
        public FakePlatformInfo(bool isWindows) => IsWindows = isWindows;

        public bool IsWindows { get; }

        public string GetDataDirectory() => Path.Combine(Path.GetTempPath(), "aieng-fake-data");

        public string GetConfigDirectory() => Path.Combine(Path.GetTempPath(), "aieng-fake-config");
    }
}
