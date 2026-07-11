namespace AiEng.Platform.Domain.Projects;

public sealed class Project
{
    public Guid Id { get; }

    public string Name { get; private set; }

    public string Path { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? LastUsedAt { get; private set; }

    public Project(Guid id, string name, string path, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Project id must not be empty.", nameof(id));
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name must not be empty.", nameof(name));
        }
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Project path must not be empty.", nameof(path));
        }

        Id = id;
        Name = name;
        Path = path;
        CreatedAt = createdAt;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Project name must not be empty.", nameof(newName));
        }
        Name = newName;
    }

    public void Touch(DateTimeOffset at)
    {
        LastUsedAt = at;
    }
}
