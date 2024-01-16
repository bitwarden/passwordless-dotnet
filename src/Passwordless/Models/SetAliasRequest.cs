using System.Collections.Generic;
using System.Linq;

namespace Passwordless.Models;

/// <summary>
/// Sets aliases for a given user.
/// </summary>
/// <param name="UserId">User ID.</param>
/// <param name="Aliases">List of user aliases to overwrite the current aliases (if any) with.</param>
/// <param name="Hashing">If you want your aliases to be available in plain text, set the <see cref="bool"/> false.</param>
public record SetAliasRequest(string UserId, IReadOnlyCollection<string> Aliases, bool Hashing = true)
{
    /// <summary>
    /// Sets a single alias for a given user, and removes any other aliases that may exist.
    /// </summary>
    public SetAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, [alias], hashing)
    {
    }

    public IReadOnlyCollection<string> Aliases { get; } = Aliases == null
        ? []
        : new HashSet<string>(Aliases.Where(x => !string.IsNullOrWhiteSpace(x)));

    /// <summary>
    /// Removes all aliases from a user.
    /// </summary>
    /// <param name="userId"></param>
    public static SetAliasRequest Empty(string userId) => new(userId, new HashSet<string>(), true);
}