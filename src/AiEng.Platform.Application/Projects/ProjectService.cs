using AiEng.Platform.Domain.Projects;

namespace AiEng.Platform.Application.Projects;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectStore _store;

    public ProjectService(IProjectStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public async Task<Result<Project>> RegisterAsync(
        string name,
        string path,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Project>.Failure(ValidationError.Required(nameof(name)));
        }
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result<Project>.Failure(ValidationError.Required(nameof(path)));
        }
        if (!Directory.Exists(path))
        {
            return Result<Project>.Failure(ValidationError.InvalidPath(nameof(path), path));
        }

        var project = new Project(Guid.NewGuid(), name.Trim(), path, DateTimeOffset.UtcNow);
        await _store.AddAsync(project, cancellationToken);
        return Result<Project>.Success(project);
    }

    public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default) =>
        _store.ListAsync(cancellationToken);

    public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        _store.GetAsync(id, cancellationToken);

    public async Task<Result<Project>> RenameAsync(
        Guid id,
        string newName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result<Project>.Failure(ValidationError.Required(nameof(newName)));
        }
        var project = await _store.GetAsync(id, cancellationToken);
        if (project is null)
        {
            return Result<Project>.Failure(ValidationError.NotFound("Project", id));
        }
        project.Rename(newName.Trim());
        await _store.UpdateAsync(project, cancellationToken);
        return Result<Project>.Success(project);
    }

    public async Task<Result<Project>> UnregisterAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var project = await _store.GetAsync(id, cancellationToken);
        if (project is null)
        {
            return Result<Project>.Failure(ValidationError.NotFound("Project", id));
        }
        await _store.RemoveAsync(id, cancellationToken);
        return Result<Project>.Success(project);
    }
}
