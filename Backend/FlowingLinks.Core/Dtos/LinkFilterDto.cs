namespace FlowingLinks.Core.Dtos;

public class LinkFilterDto
{
    /// <summary>
    /// Filter by description (contains search)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Filter by label IDs (array of label IDs)
    /// </summary>
    public int[]? LabelIds { get; set; }

    /// <summary>
    /// Filter by favorite status
    /// </summary>
    public FavoriteFilterOption Favorite { get; set; } = FavoriteFilterOption.All;
}

public enum FavoriteFilterOption
{
    /// <summary>
    /// Show all links (no favorite filter)
    /// </summary>
    All = 0,

    /// <summary>
    /// Show only favorite links
    /// </summary>
    FavoritesOnly = 1,

    /// <summary>
    /// Show only non-favorite links
    /// </summary>
    NonFavoritesOnly = 2
} 