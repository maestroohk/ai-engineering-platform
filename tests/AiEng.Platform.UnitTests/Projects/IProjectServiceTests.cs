using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using Xunit;

namespace AiEng.Platform.UnitTests.Projects;

public class IProjectServiceTests
{
    [Fact]
    public async Task RegisterAsync_returns_failure_when_name_is_whitespace()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var result = await service.RegisterAsync("   ", "/tmp/x");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("required", result.Error!.Code);
        Assert.Equal("name", result.Error!.Message.Split(' ')[0]);
    }

    [Fact]
    public async Task RegisterAsync_returns_failure_when_path_is_whitespace()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var result = await service.RegisterAsync("alpha", "   ");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("required", result.Error!.Code);
    }

    [Fact]
    public async Task RegisterAsync_returns_failure_when_path_does_not_exist()
    {
        var service = new ProjectService(new InMemoryProjectStore());
        var missing = Path.Combine(Path.GetTempPath(), "aieng-missing-" + Guid.NewGuid().ToString("N"));
        if (Directory.Exists(missing))
        {
            Directory.Delete(missing, recursive: true);
        }

        var result = await service.RegisterAsync("alpha", missing);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("invalid_path", result.Error!.Code);
    }

    [Fact]
    public async Task RegisterAsync_returns_project_with_trimmed_name_and_persists_it()
    {
        var dir = CreateTempDirectory();
        try
        {
            var store = new InMemoryProjectStore();
            var service = new ProjectService(store);

            var result = await service.RegisterAsync("  alpha  ", dir);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("alpha", result.Value!.Name);
            Assert.Equal(dir, result.Value.Path);
            var listed = await service.ListAsync();
            Assert.Single(listed);
            Assert.Equal(result.Value.Id, listed[0].Id);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task ListAsync_returns_empty_collection_when_no_projects_registered()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var projects = await service.ListAsync();

        Assert.Empty(projects);
    }

    [Fact]
    public async Task GetAsync_returns_project_when_registered_and_null_otherwise()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var missing = await service.GetAsync(Guid.NewGuid());

        Assert.Null(missing);
    }

    [Fact]
    public async Task RenameAsync_returns_failure_when_name_is_whitespace()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var result = await service.RenameAsync(Guid.NewGuid(), "   ");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("required", result.Error!.Code);
    }

    [Fact]
    public async Task RenameAsync_returns_not_found_when_project_does_not_exist()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var result = await service.RenameAsync(Guid.NewGuid(), "renamed");

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("not_found", result.Error!.Code);
    }

    [Fact]
    public async Task RenameAsync_persists_renamed_project()
    {
        var dir = CreateTempDirectory();
        try
        {
            var service = new ProjectService(new InMemoryProjectStore());
            var registered = await service.RegisterAsync("alpha", dir);
            Assert.True(registered.IsSuccess);

            var renamed = await service.RenameAsync(registered.Value!.Id, "  alpha-renamed  ");

            Assert.True(renamed.IsSuccess);
            Assert.Equal("alpha-renamed", renamed.Value!.Name);
            var current = await service.GetAsync(registered.Value.Id);
            Assert.NotNull(current);
            Assert.Equal("alpha-renamed", current!.Name);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task UnregisterAsync_returns_not_found_when_project_does_not_exist()
    {
        var service = new ProjectService(new InMemoryProjectStore());

        var result = await service.UnregisterAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("not_found", result.Error!.Code);
    }

    [Fact]
    public async Task UnregisterAsync_removes_the_project_from_the_store()
    {
        var dir = CreateTempDirectory();
        try
        {
            var service = new ProjectService(new InMemoryProjectStore());
            var registered = await service.RegisterAsync("alpha", dir);
            Assert.True(registered.IsSuccess);

            var result = await service.UnregisterAsync(registered.Value!.Id);

            Assert.True(result.IsSuccess);
            Assert.Null(await service.GetAsync(registered.Value.Id));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task Constructor_throws_when_store_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => new ProjectService(null!));
    }

    [Fact]
    public void Project_constructor_rejects_empty_id()
    {
        Assert.Throws<ArgumentException>(() => new Project(Guid.Empty, "alpha", "/tmp/alpha", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Project_constructor_rejects_empty_name()
    {
        Assert.Throws<ArgumentException>(() => new Project(Guid.NewGuid(), "   ", "/tmp/alpha", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Project_constructor_rejects_empty_path()
    {
        Assert.Throws<ArgumentException>(() => new Project(Guid.NewGuid(), "alpha", "  ", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Project_Rename_rejects_empty_name()
    {
        var project = new Project(Guid.NewGuid(), "alpha", "/tmp/alpha", DateTimeOffset.UtcNow);

        Assert.Throws<ArgumentException>(() => project.Rename("   "));
    }

    [Fact]
    public void Project_Touch_records_last_used_at()
    {
        var project = new Project(Guid.NewGuid(), "alpha", "/tmp/alpha", DateTimeOffset.UtcNow);
        Assert.Null(project.LastUsedAt);

        var at = DateTimeOffset.UtcNow;
        project.Touch(at);

        Assert.Equal(at, project.LastUsedAt);
    }

    private static string CreateTempDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), "aieng-svc-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
