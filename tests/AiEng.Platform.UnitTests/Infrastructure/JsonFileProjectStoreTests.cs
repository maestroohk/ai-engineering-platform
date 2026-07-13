using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using AiEng.Platform.Infrastructure.Platform;
using AiEng.Platform.Infrastructure.Projects;
using Xunit;

namespace AiEng.Platform.UnitTests.Infrastructure;

public class JsonFileProjectStoreTests : IDisposable
{
    private readonly string _dataDir;
    private readonly JsonFileProjectStore _store;

    public JsonFileProjectStoreTests()
    {
        _dataDir = CreateTempDataDirectory();
        var options = new JsonFileProjectStoreOptions { DataDirectory = _dataDir };
        _store = new JsonFileProjectStore(new SystemPlatformInfo(), options);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dataDir))
        {
            try
            {
                Directory.Delete(_dataDir, recursive: true);
            }
            catch
            {
            }
        }
    }

    [Fact]
    public async Task ListAsync_returns_empty_collection_when_file_does_not_exist()
    {
        var projects = await _store.ListAsync();
        Assert.Empty(projects);
    }

    [Fact]
    public async Task AddAsync_then_ListAsync_round_trips_a_project()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        await _store.AddAsync(project);
        var projects = await _store.ListAsync();

        Assert.Single(projects);
        Assert.Equal(project.Id, projects[0].Id);
        Assert.Equal("alpha", projects[0].Name);
    }

    [Fact]
    public async Task ListAsync_orders_projects_by_name_ordinal_ignore_case()
    {
        await _store.AddAsync(NewProject("charlie", "/tmp/charlie"));
        await _store.AddAsync(NewProject("Alpha", "/tmp/alpha"));
        await _store.AddAsync(NewProject("bravo", "/tmp/bravo"));

        var projects = await _store.ListAsync();

        Assert.Equal(new[] { "Alpha", "bravo", "charlie" }, projects.Select(static p => p.Name).ToArray());
    }

    [Fact]
    public async Task GetAsync_returns_project_when_present_and_null_when_absent()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        var found = await _store.GetAsync(project.Id);
        var missing = await _store.GetAsync(Guid.NewGuid());

        Assert.NotNull(found);
        Assert.Equal(project.Id, found!.Id);
        Assert.Null(missing);
    }

    [Fact]
    public async Task AddAsync_throws_when_duplicate_id_is_added()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _store.AddAsync(project));
        Assert.Contains("already registered", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateAsync_replaces_the_stored_project()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        project.Rename("alpha-renamed");
        await _store.UpdateAsync(project);

        var found = await _store.GetAsync(project.Id);
        Assert.NotNull(found);
        Assert.Equal("alpha-renamed", found!.Name);
    }

    [Fact]
    public async Task UpdateAsync_adds_project_when_id_is_new()
    {
        var project = NewProject("alpha", "/tmp/alpha");

        await _store.UpdateAsync(project);

        var found = await _store.GetAsync(project.Id);
        Assert.NotNull(found);
    }

    [Fact]
    public async Task RemoveAsync_removes_the_project()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        await _store.RemoveAsync(project.Id);

        Assert.Null(await _store.GetAsync(project.Id));
        Assert.Empty(await _store.ListAsync());
    }

    [Fact]
    public async Task RemoveAsync_is_a_no_op_when_project_is_absent()
    {
        await _store.RemoveAsync(Guid.NewGuid());
        Assert.Empty(await _store.ListAsync());
    }

    [Fact]
    public async Task AddAsync_throws_when_project_is_null()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _store.AddAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_throws_when_project_is_null()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _store.UpdateAsync(null!));
    }

    [Fact]
    public async Task ListAsync_returns_empty_collection_when_file_is_corrupt()
    {
        File.WriteAllText(Path.Combine(_dataDir, "projects.json"), "{not valid json");

        var projects = await _store.ListAsync();

        Assert.Empty(projects);
    }

    [Fact]
    public async Task ListAsync_recovers_from_corrupt_file_and_can_add_new_projects()
    {
        File.WriteAllText(Path.Combine(_dataDir, "projects.json"), "{not valid json");

        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        var projects = await _store.ListAsync();
        Assert.Single(projects);
        Assert.Equal(project.Id, projects[0].Id);
    }

    [Fact]
    public async Task GetAsync_returns_null_when_file_is_corrupt()
    {
        File.WriteAllText(Path.Combine(_dataDir, "projects.json"), "{not valid json");

        var project = await _store.GetAsync(Guid.NewGuid());

        Assert.Null(project);
    }

    [Fact]
    public async Task Store_persists_projects_to_disk_across_instances()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        await _store.AddAsync(project);

        var reopened = new JsonFileProjectStore(
            new SystemPlatformInfo(),
            new JsonFileProjectStoreOptions { DataDirectory = _dataDir });

        var projects = await reopened.ListAsync();

        Assert.Single(projects);
        Assert.Equal(project.Id, projects[0].Id);
    }

    [Fact]
    public async Task ListAsync_concurrent_adds_keep_store_consistent()
    {
        var ids = Enumerable.Range(0, 25).Select(static i => NewProject($"p-{i:D2}", "/tmp/p-" + i)).ToArray();
        var tasks = new Task[ids.Length];
        for (var i = 0; i < ids.Length; i++)
        {
            tasks[i] = _store.AddAsync(ids[i]);
        }
        await Task.WhenAll(tasks);

        var listed = await _store.ListAsync();

        Assert.Equal(25, listed.Count);
        Assert.Equal(
            new HashSet<Guid>(ids.Select(static i => i.Id)),
            new HashSet<Guid>(listed.Select(static p => p.Id)));
    }

    [Fact]
    public async Task ListAsync_concurrent_add_and_remove_keep_store_consistent()
    {
        var seed = Enumerable.Range(0, 20).Select(static i => NewProject($"p-{i:D2}", "/tmp/p-" + i)).ToArray();
        var seedTasks = new Task[seed.Length];
        for (var i = 0; i < seed.Length; i++)
        {
            seedTasks[i] = _store.AddAsync(seed[i]);
        }
        await Task.WhenAll(seedTasks);

        var addTasks = new List<Task>();
        for (var i = 100; i < 110; i++)
        {
            addTasks.Add(_store.AddAsync(NewProject($"extra-{i:D3}", "/tmp/extra-" + i)));
        }
        var removeTasks = new List<Task>();
        for (var i = 0; i < 5; i++)
        {
            removeTasks.Add(_store.RemoveAsync(seed[i].Id));
        }
        await Task.WhenAll(addTasks.Concat(removeTasks));

        var listed = await _store.ListAsync();
        Assert.Equal(seed.Length - 5 + 10, listed.Count);
    }

    [Fact]
    public async Task Cancellation_throws_when_token_is_cancelled_before_add()
    {
        var project = NewProject("alpha", "/tmp/alpha");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await _store.AddAsync(project, cts.Token));
    }

    [Fact]
    public void Constructor_throws_when_platformInfo_is_null()
    {
        Assert.Throws<ArgumentNullException>(
            () => new JsonFileProjectStore(null!));
    }

    [Fact]
    public void Constructor_uses_default_options_when_options_is_null()
    {
        var store = new JsonFileProjectStore(new SystemPlatformInfo(), null);

        Assert.NotNull(store);
    }

    [Fact]
    public void Constructor_creates_data_directory_when_it_does_not_exist()
    {
        var nested = Path.Combine(_dataDir, "nested", "deeper");
        var options = new JsonFileProjectStoreOptions { DataDirectory = nested };

        var store = new JsonFileProjectStore(new SystemPlatformInfo(), options);

        Assert.True(Directory.Exists(nested));
    }

    private static string CreateTempDataDirectory()
    {
        var dir = Path.Combine(
            Path.GetTempPath(),
            "aieng-infra-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, DateTimeOffset.UtcNow);
}
