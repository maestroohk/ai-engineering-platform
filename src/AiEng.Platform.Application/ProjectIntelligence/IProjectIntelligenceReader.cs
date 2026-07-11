namespace AiEng.Platform.Application.ProjectIntelligence;

public interface IProjectIntelligenceReader
{
    Task<ProjectIntelligenceSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
