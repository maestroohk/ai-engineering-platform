using AiEng.Platform.Application.Projects;
using AiEng.Platform.Domain.Projects;
using AiEng.Platform.UnitTests.Infrastructure;
using Xunit;

namespace AiEng.Platform.UnitTests.Projects;

public class InMemoryProjectStoreTests
{
    [Fact]
    public async Task ListAsync_returns_empty_snapshot_when_no_projects_are_added()
    {
        var store = new InMemoryProjectStore();

        var projects = await store.ListAsync();

        Assert.Empty(projects);
    }

    [Fact]
    public async Task AddAsync_then_ListAsync_round_trips_a_project()
    {
        var store = new InMemoryProjectStore();
        var project = NewProject("alpha", "/tmp/alpha");

        await store.AddAsync(project);
        var projects = await store.ListAsync();

        Assert.Single(projects);
        Assert.Equal(project.Id, projects[0].Id);
        Assert.Equal("alpha", projects[0].Name);
    }

    [Fact]
    public async Task ListAsync_orders_projects_by_name_ordinal_ignore_case()
    {
        var store = new InMemoryProjectStore();
        await store.AddAsync(NewProject("charlie", "/tmp/charlie"));
        await store.AddAsync(NewProject("Alpha", "/tmp/alpha"));
        await store.AddAsync(NewProject("bravo", "/tmp/bravo"));

        var projects = await store.ListAsync();

        Assert.Equal(new[] { "Alpha", "bravo", "charlie" }, projects.Select(static p => p.Name).ToArray());
    }

    [Fact]
    public async Task GetAsync_returns_the_project_when_present_and_null_when_absent()
    {
        var store = new InMemoryProjectStore();
        var project = NewProject("alpha", "/tmp/alpha");
        await store.AddAsync(project);

        var found = await store.GetAsync(project.Id);
        var missing = await store.GetAsync(Guid.NewGuid());

        Assert.NotNull(found);
        Assert.Equal(project.Id, found!.Id);
        Assert.Null(missing);
    }

    [Fact]
    public async Task AddAsync_throws_when_duplicate_id_is_added()
    {
        var store = new InMemoryProjectStore();
        var project = NewProject("alpha", "/tmp/alpha");
        await store.AddAsync(project);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await store.AddAsync(project));
        Assert.Contains("already registered", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateAsync_replaces_the_stored_project()
    {
        var store = new InMemoryProjectStore();
        var project = NewProject("alpha", "/tmp/alpha");
        await store.AddAsync(project);

        project.Rename("alpha-renamed");
        await store.UpdateAsync(project);

        var found = await store.GetAsync(project.Id);
        Assert.NotNull(found);
        Assert.Equal("alpha-renamed", found!.Name);
    }

    [Fact]
    public async Task RemoveAsync_removes_the_project()
    {
        var store = new InMemoryProjectStore();
        var project = NewProject("alpha", "/tmp/alpha");
        await store.AddAsync(project);

        await store.RemoveAsync(project.Id);

        Assert.Null(await store.GetAsync(project.Id));
        Assert.Empty(await store.ListAsync());
    }

    [Fact]
    public async Task RemoveAsync_is_a_no_op_when_project_is_absent()
    {
        var store = new InMemoryProjectStore();

        await store.RemoveAsync(Guid.NewGuid());

        Assert.Empty(await store.ListAsync());
    }

    [Fact]
    public async Task AddAsync_throws_when_project_is_null()
    {
        var store = new InMemoryProjectStore();

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await store.AddAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_throws_when_project_is_null()
    {
        var store = new InMemoryProjectStore();

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await store.UpdateAsync(null!));
    }

    [Fact]
    public async Task ListAsync_concurrent_adds_keep_store_consistent()
    {
        var store = new InMemoryProjectStore();
        var ids = Enumerable.Range(0, 50).Select(static i => NewProject($"p-{i:D2}", "/tmp/p-" + i)).ToArray();
        await Task.WhenAll(ids.Select(id => store.AddAsync(id)));

        var listed = await store.ListAsync();

        Assert.Equal(50, listed.Count);
        Assert.Equal(new HashSet<Guid>(ids.Select(static i => i.Id)), new HashSet<Guid>(listed.Select(static p => p.Id)));
    }

    private static Project NewProject(string name, string path) =>
        new(Guid.NewGuid(), name, path, DateTimeOffset.UtcNow);
}
