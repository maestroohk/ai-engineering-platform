using System.Collections.Concurrent;
using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;

namespace AiEng.Platform.UnitTests.Infrastructure;

public sealed class InMemoryProjectStore : IProjectStore
{
    private readonly ConcurrentDictionary<Guid, Project> _projects = new();

    public Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Project> snapshot = _projects.Values
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _projects.TryGetValue(id, out var project);
        return Task.FromResult(project);
    }

    public Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }
        if (!_projects.TryAdd(project.Id, project))
        {
            throw new InvalidOperationException(
                $"A project with id {project.Id} is already registered.");
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project is null)
        {
            throw new ArgumentNullException(nameof(project));
        }
        _projects[project.Id] = project;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _projects.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
