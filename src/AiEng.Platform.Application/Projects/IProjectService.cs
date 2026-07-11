using AiEng.Platform.Domain.Projects;

namespace AiEng.Platform.Application.Projects;

public interface IProjectService
{
    Task<Result<Project>> RegisterAsync(string name, string path, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> ListAsync(CancellationToken cancellationToken = default);

    Task<Project?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<Project>> RenameAsync(Guid id, string newName, CancellationToken cancellationToken = default);

    Task<Result<Project>> UnregisterAsync(Guid id, CancellationToken cancellationToken = default);
}
