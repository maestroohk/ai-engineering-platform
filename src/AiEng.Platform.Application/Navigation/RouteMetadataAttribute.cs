namespace AiEng.Platform.Application.Navigation;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RouteMetadataAttribute : Attribute
{
    public string Href { get; }
    public string Title { get; }
    public int Order { get; init; }
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Parent { get; init; }
    public string? BadgeText { get; init; }
    public bool ShowInSidebar { get; init; }
    public bool MatchPrefix { get; init; }

    public RouteMetadataAttribute(
        string href,
        string title,
        int order = 0,
        string? description = null,
        string? icon = null,
        string? parent = null,
        string? badgeText = null,
        bool showInSidebar = true,
        bool matchPrefix = false)
    {
        Href = href;
        Title = title;
        Order = order;
        Description = description;
        Icon = icon;
        Parent = parent;
        BadgeText = badgeText;
        ShowInSidebar = showInSidebar;
        MatchPrefix = matchPrefix;
    }

    public RouteMetadata ToMetadata() => new(
        Href,
        Title,
        Order,
        Description,
        Icon,
        Parent,
        BadgeText,
        ShowInSidebar,
        MatchPrefix);
}
