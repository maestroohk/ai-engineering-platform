namespace AiEng.Platform.Application.Navigation;

public interface INavigationRegistry
{
    IReadOnlyList<RouteMetadata> Routes { get; }

    RouteMetadata? FindByHref(string href);

    IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref);
}
