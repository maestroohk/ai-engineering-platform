namespace AiEng.Platform.Application.Navigation;

public sealed record RouteMetadata(
    string Href,
    string Title,
    int Order,
    string? Description,
    string? Icon,
    string? Parent,
    string? BadgeText,
    bool ShowInSidebar = true,
    bool MatchPrefix = false);
