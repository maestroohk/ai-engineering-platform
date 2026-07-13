using AiEng.Platform.App.Components;
using AiEng.Platform.App.Composition;
using AiEng.Platform.Application.Capabilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddPlatformServices(typeof(Program).Assembly);

var app = builder.Build();

await LogHostCapabilitiesAsync(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task LogHostCapabilitiesAsync(WebApplication app)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var service = app.Services.GetRequiredService<IHostCapabilitiesService>();
        var report = await service.DetectAsync(cts.Token);
        var tools = string.Join("; ", report.Capabilities
            .Where(c => !c.Key.StartsWith("provider:", StringComparison.Ordinal))
            .Select(c => $"{c.Key}={(c.Available ? "Available" : "Unavailable")}{(string.IsNullOrEmpty(c.Version) ? string.Empty : " " + c.Version)}"));
        var credentials = string.Join(", ", report.Capabilities
            .Where(c => c.Key.StartsWith("provider:", StringComparison.Ordinal))
            .Select(c => $"{c.Key}={(c.CredentialAvailable ? "set" : "not set")}"));
        logger.LogInformation(
            "Host capability report at {DetectedAt:o}: tools: {Tools}; credentials: {Credentials}",
            report.DetectedAt, tools, credentials);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to detect host capabilities at startup.");
    }
}
