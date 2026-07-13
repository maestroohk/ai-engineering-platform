using System.Text.Json;
using System.Text.Json.Serialization;
using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AiEng.Platform.Infrastructure.Projects;

public sealed class JsonFileProjectStore : IProjectStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private readonly ILogger<JsonFileProjectStore> _logger;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public JsonFileProjectStore(
        IPlatformInfo platformInfo,
        JsonFileProjectStoreOptions? options = null,
        ILogger<JsonFileProjectStore>? logger = null)
    {
        if (platformInfo is null)
        {
            throw new ArgumentNullException(nameof(platformInfo));
        }

        var effective = options ?? JsonFileProjectStoreOptions.Default;
        var dataDir = effective.DataDirectory ?? platformInfo.GetDataDirectory();
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, effective.FileName);
        _logger = logger ?? NullLogger<JsonFileProjectStore>.Instance;
    }

    public async Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
    {
        var projects = await ReadSnapshotAsync(cancellationToken);
        return projects
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var projects = await ReadSnapshotAsync(cancellationToken);
        return projects.FirstOrDefault(p => p.Id == id);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            var projects = await ReadSnapshotNoLockAsync(cancellationToken);
            if (projects.Any(p => p.Id == project.Id))
            {
                throw new InvalidOperationException(
                    $"A project with id {project.Id} is already registered.");
            }
            projects.Add(project);
            await WriteSnapshotNoLockAsync(projects, cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            var projects = await ReadSnapshotNoLockAsync(cancellationToken);
            var index = projects.FindIndex(p => p.Id == project.Id);
            if (index >= 0)
            {
                projects[index] = project;
            }
            else
            {
                projects.Add(project);
            }
            await WriteSnapshotNoLockAsync(projects, cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            var projects = await ReadSnapshotNoLockAsync(cancellationToken);
            projects.RemoveAll(p => p.Id == id);
            await WriteSnapshotNoLockAsync(projects, cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task<List<Project>> ReadSnapshotAsync(CancellationToken cancellationToken)
    {
        await _writeLock.WaitAsync(cancellationToken);
        try
        {
            return await ReadSnapshotNoLockAsync(cancellationToken);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private async Task<List<Project>> ReadSnapshotNoLockAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!File.Exists(_filePath))
        {
            return new List<Project>();
        }
        try
        {
            await using var stream = File.OpenRead(_filePath);
            var projects = await JsonSerializer.DeserializeAsync<List<Project>>(
                stream,
                SerializerOptions,
                cancellationToken);
            return projects ?? new List<Project>();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(
                ex,
                "Projects store at {FilePath} is corrupt; returning an empty project list.",
                _filePath);
            return new List<Project>();
        }
    }

    private async Task WriteSnapshotNoLockAsync(
        IReadOnlyList<Project> projects,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tempPath = _filePath + ".tmp-" + Guid.NewGuid().ToString("N");
        try
        {
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    projects.ToList(),
                    SerializerOptions,
                    cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            if (File.Exists(_filePath))
            {
                File.Replace(tempPath, _filePath, destinationBackupFileName: null, ignoreMetadataErrors: true);
            }
            else
            {
                File.Move(tempPath, _filePath);
            }
        }
        catch
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            throw;
        }
    }
}
