using FlowingLinks.Core.Models;

namespace FlowingLinks.Core.Utils;

internal static class QueryExtension
{
    internal static IQueryable<Link> FilterByUser(this IQueryable<Link> query, int userId) =>
        query.Where(l => l.UserId == userId);
}