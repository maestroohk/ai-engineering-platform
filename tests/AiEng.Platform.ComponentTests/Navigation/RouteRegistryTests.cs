using AiEng.Platform.Application.Navigation;
using Xunit;

namespace AiEng.Platform.ComponentTests.Navigation;

public class RouteRegistryTests
{
    private static readonly AssemblyMarkerAccess Markers = new();

    [Fact]
    public void Empty_assembly_list_returns_empty_routes()
    {
        var registry = new RouteRegistry();

        Assert.Empty(registry.Routes);
    }

    [Fact]
    public void Attribute_on_a_page_produces_a_route_with_the_matching_fields()
    {
        var registry = new RouteRegistry(typeof(SingleRouteMarker).Assembly);

        var single = registry.Routes.SingleOrDefault(static r => r.Href == "/single");
        Assert.NotNull(single);
        Assert.Equal("Single", single!.Title);
        Assert.Equal(7, single.Order);
    }

    [Fact]
    public void Multiple_attributes_are_returned_sorted_by_Order_then_Title()
    {
        var registry = new RouteRegistry(typeof(SingleRouteMarker).Assembly);

        var orderedHrefs = registry.Routes
            .Where(static r => r.Href is "/first" or "/second" or "/third" or "/single")
            .Select(static r => r.Href)
            .ToArray();
        Assert.Equal(new[] { "/first", "/second", "/third", "/single" }, orderedHrefs);
    }

    [Fact]
    public void FindByHref_returns_the_matching_entry()
    {
        var registry = new RouteRegistry(typeof(SingleRouteMarker).Assembly);

        var found = registry.FindByHref("/second");

        Assert.NotNull(found);
        Assert.Equal("Second", found!.Title);
        Assert.Equal(1, found.Order);
    }

    [Fact]
    public void FindByHref_returns_null_for_unknown_or_empty_href()
    {
        var registry = new RouteRegistry(typeof(SingleRouteMarker).Assembly);

        Assert.Null(registry.FindByHref("/nope"));
        Assert.Null(registry.FindByHref(string.Empty));
        Assert.Null(registry.FindByHref("   "));
    }

    [Fact]
    public void ChildrenOf_returns_children_for_a_parent_and_top_level_for_null()
    {
        var registry = new RouteRegistry(typeof(SingleRouteMarker).Assembly);

        var topLevel = registry.ChildrenOf(null);
        Assert.Contains(topLevel, static r => r.Href == "/admin");

        var childrenOfAdmin = registry.ChildrenOf("/admin");
        Assert.Equal(2, childrenOfAdmin.Count);
        Assert.All(childrenOfAdmin, static r => Assert.Equal("/admin", r.Parent));

        var empty = registry.ChildrenOf("/nope");
        Assert.Empty(empty);
    }

    [RouteMetadata("/single", "Single", Order = 7)]
    private sealed class SingleRouteMarker;

    [RouteMetadata("/first", "First", Order = 0)]
    private sealed class MultiFirstMarker;

    [RouteMetadata("/second", "Second", Order = 1)]
    private sealed class MultiSecondMarker;

    [RouteMetadata("/third", "Third", Order = 2)]
    private sealed class MultiThirdMarker;

    [RouteMetadata("/admin", "Admin", Order = 0)]
    private sealed class AdminMarker;

    [RouteMetadata("/admin/users", "Users", Order = 0, Parent = "/admin")]
    private sealed class AdminUsersMarker;

    [RouteMetadata("/admin/groups", "Groups", Order = 1, Parent = "/admin")]
    private sealed class AdminGroupsMarker;

    private sealed class AssemblyMarkerAccess;
}
