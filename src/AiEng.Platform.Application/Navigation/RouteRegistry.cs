using System.Reflection;

namespace AiEng.Platform.Application.Navigation;

public sealed class RouteRegistry : INavigationRegistry
{
    private readonly IReadOnlyList<RouteMetadata> _routes;
    private readonly IReadOnlyDictionary<string, RouteMetadata> _byHref;

    public RouteRegistry(params Assembly[] scanAssemblies)
    {
        if (scanAssemblies is null || scanAssemblies.Length == 0)
        {
            _routes = Array.Empty<RouteMetadata>();
            _byHref = new Dictionary<string, RouteMetadata>(StringComparer.OrdinalIgnoreCase);
            return;
        }

        var collected = new List<RouteMetadata>();
        foreach (var assembly in scanAssemblies)
        {
            if (assembly is null)
            {
                continue;
            }

            foreach (var type in SafeGetTypes(assembly))
            {
                var attribute = type.GetCustomAttribute<RouteMetadataAttribute>();
                if (attribute is null)
                {
                    continue;
                }

                collected.Add(attribute.ToMetadata());
            }
        }

        var sorted = collected
            .OrderBy(static m => m.Order)
            .ThenBy(static m => m.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        _routes = sorted;
        _byHref = sorted.ToDictionary(static m => m.Href, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<RouteMetadata> Routes => _routes;

    public RouteMetadata? FindByHref(string href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            return null;
        }

        return _byHref.TryGetValue(href, out var match) ? match : null;
    }

    public IReadOnlyList<RouteMetadata> ChildrenOf(string? parentHref)
    {
        if (string.IsNullOrEmpty(parentHref))
        {
            return _routes.Where(static m => string.IsNullOrEmpty(m.Parent)).ToList();
        }

        return _routes.Where(m => string.Equals(m.Parent, parentHref, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private static Type[] SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(static t => t is not null).Cast<Type>().ToArray();
        }
    }
}
