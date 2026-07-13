using AiEng.Platform.Application.Infrastructure;

namespace AiEng.Platform.Infrastructure.Projects;

public sealed class JsonFileProjectStoreOptions
{
    public const string DefaultFileName = "projects.json";

    public string? DataDirectory { get; set; }

    public string FileName { get; set; } = DefaultFileName;

    public static JsonFileProjectStoreOptions Default { get; } = new();
}
